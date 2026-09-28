"""Extrae piezas de los GLB CC0 originales; conserva procedencia y materiales.
No crea geometría sustitutiva: exporta triángulos existentes a OBJ.
"""
import json, math, pathlib, struct

ROOT = pathlib.Path(__file__).resolve().parents[1] / 'Assets' / 'Terceros'
IDENTITY = [1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1]

def mul(a,b):
    return [sum(a[k*4+r]*b[c*4+k] for k in range(4)) for c in range(4) for r in range(4)]

def point(m,p):
    return tuple(sum(m[k*4+r]*p[k] for k in range(3))+m[12+r] for r in range(3))

def flatten(path):
    data=path.read_bytes(); size=struct.unpack_from('<I',data,12)[0]; j=json.loads(data[20:20+size]); binary=data[28+size:]
    def accessor(i):
        a=j['accessors'][i];v=j['bufferViews'][a['bufferView']];fmt,n={5120:('b',1),5121:('B',1),5122:('h',2),5123:('H',2),5125:('I',4),5126:('f',4)}[a['componentType']]
        count={'SCALAR':1,'VEC2':2,'VEC3':3,'VEC4':4}[a['type']]; stride=v.get('byteStride',n*count);start=v.get('byteOffset',0)+a.get('byteOffset',0)
        result=[struct.unpack_from('<'+fmt*count,binary,start+k*stride) for k in range(a['count'])]
        if a.get('normalized'):
            divisor={5120:127,5121:255,5122:32767,5123:65535}[a['componentType']]
            result=[tuple(max(-1,x/divisor) for x in row) for row in result]
        return result
    parts=[]
    def walk(i,parent):
        node=j['nodes'][i];m=node.get('matrix')
        if m is None:
            x,y,z,w=node.get('rotation',[0,0,0,1]);sx,sy,sz=node.get('scale',[1,1,1]);tx,ty,tz=node.get('translation',[0,0,0])
            m=[(1-2*y*y-2*z*z)*sx,(2*x*y+2*z*w)*sx,(2*x*z-2*y*w)*sx,0,(2*x*y-2*z*w)*sy,(1-2*x*x-2*z*z)*sy,(2*y*z+2*x*w)*sy,0,(2*x*z+2*y*w)*sz,(2*y*z-2*x*w)*sz,(1-2*x*x-2*y*y)*sz,0,tx,ty,tz,1]
        world=mul(parent,m)
        if 'mesh' in node:
            for primitive in j['meshes'][node['mesh']]['primitives']:
                assert primitive.get('mode',4)==4
                vertices=[point(world,p) for p in accessor(primitive['attributes']['POSITION'])]
                indices=[v[0] for v in accessor(primitive['indices'])] if 'indices' in primitive else list(range(len(vertices)))
                material=j['materials'][primitive.get('material',0)]
                parts.append((node.get('name','mesh'),vertices,[indices[k:k+3] for k in range(0,len(indices),3)],material))
        for child in node.get('children',[]):walk(child,world)
    for node in j['scenes'][j.get('scene',0)]['nodes']:walk(node,IDENTITY)
    return parts

def components(parts):
    out=[]
    for name,vs,fs,mat in parts:
        groups={}; vertex_tri={}; parents=list(range(len(fs)))
        def find(i):
            while parents[i]!=i:parents[i]=parents[parents[i]];i=parents[i]
            return i
        for i,f in enumerate(fs):
            for index in f:
                key=tuple(round(x,5) for x in vs[index])
                if key in vertex_tri:parents[find(i)]=find(vertex_tri[key])
                else:vertex_tri[key]=i
        for i,f in enumerate(fs):groups.setdefault(find(i),[]).append(f)
        for faces in groups.values():
            coords=[vs[i] for f in faces for i in f]
            lo=tuple(min(p[k] for p in coords) for k in range(3)); hi=tuple(max(p[k] for p in coords) for k in range(3))
            out.append(dict(name=name,vertices=vs,faces=faces,material=mat,lo=lo,hi=hi,center=tuple((a+b)/2 for a,b in zip(lo,hi))))
    return out

def export(parts,path,predicate=lambda p:True,transform=lambda p:p):
    path.parent.mkdir(exist_ok=True);lines=['# Piezas extraídas de modelos de terceros; véase procedencia.json','mtllib '+path.with_suffix('.mtl').name];ml=[]; offset=1; kept=0
    for n,p in enumerate(parts):
        if not predicate(p):continue
        kept+=1;ids=sorted(set(i for f in p['faces'] for i in f));mapping={v:i+offset for i,v in enumerate(ids)}
        lines.extend(['o pieza_'+str(n),'usemtl material_'+str(n)])
        for i in ids:lines.append('v '+' '.join(str(v) for v in transform(p['vertices'][i])))
        for f in p['faces']:lines.append('f '+' '.join(str(mapping[i]) for i in f))
        color=p['material'].get('pbrMetallicRoughness',{}).get('baseColorFactor',[1,1,1,1])
        ml.extend(['newmtl material_'+str(n),'Kd '+' '.join(str(x) for x in color[:3])]); offset+=len(ids)
    assert kept, path
    path.write_text('\n'.join(lines)+'\n');path.with_suffix('.mtl').write_text('\n'.join(ml)+'\n')

if __name__=='__main__':
    import sys
    for arg in sys.argv[1:]:
        ps=components(flatten(ROOT/arg))
        print(arg)
        for i,p in enumerate(ps):print(i,p['name'],len(p['faces']), 'lo',tuple(round(x,4) for x in p['lo']),'hi',tuple(round(x,4) for x in p['hi']))


def preparar():
    def piezas(relative):return components(flatten(ROOT/relative))
    def guardar(ps,relative,indices):export([ps[i] for i in indices],ROOT/relative)
    ps=piezas('dental-practice/instrument-tray.glb')
    guardar(ps,'dental-practice/espejo.obj',[2,3,7])
    guardar(ps,'dental-practice/explorador.obj',[4])
    guardar(ps,'dental-practice/algodon.obj',[8,9,10])
    guardar(ps,'dental-practice/charola.obj',[0,1,6])
    ps=piezas('gallery-gloves/gloves.glb')
    guardar(ps,'gallery-gloves/guantes.obj',[5,6])
    ps=piezas('syringe-jtoastie/syringe.glb')
    export([ps[i] for i in [35,36]],ROOT/'syringe-jtoastie/aguja.obj',transform=lambda v:(v[0],v[2],v[1]))
    export(ps,ROOT/'syringe-jtoastie/jeringa.obj',transform=lambda v:(v[0],v[2],v[1]))
    ps=piezas('tattoo-studio/rolling-tray-table.glb')
    export(ps,ROOT/'tattoo-studio/carrito-vacio.obj',lambda p:p['material'].get('name')=='steel' or p['name'].startswith('wheel'))
    ps=piezas('dental-practice/autoclave-bench-open.glb')
    export(ps,ROOT/'dental-practice/autoclave.obj',lambda p:p['lo'][1]>.9)
    ps=piezas('dental-practice/surgery-door-module-open.glb')
    export(ps,ROOT/'dental-practice/puerta.obj',lambda p:p['name'].startswith('door-'))
    ps=piezas('tattoo-studio/barrier-film-roll.glb')
    # La hoja colgante se separa del dispensador; se escala para cada barrera.
    export(ps,ROOT/'tattoo-studio/hoja-barrera.obj',lambda p:p['material'].get('name')=='teal' and p['hi'][1]-p['lo'][1]>.08 and p['hi'][2]-p['lo'][2]<.02)
    ps=piezas('dental-practice/intraoral-xray-arm.glb')
    guardar(ps,'dental-practice/soporte-rayos.obj',[0,1,2,3])
    export(ps,ROOT/'dental-practice/brazo-rayos.obj',lambda p:p['name'].startswith('xray-arm'),transform=lambda v:(v[0],v[1]-.27,v[2]-.02))
    ps=piezas('dental-practice/reception-desk.glb')
    guardar(ps,'dental-practice/mostrador.obj',list(range(6)))
    ps=piezas('scalpel-google/scalpel.glb')
    hoja=ps[0].copy()
    hoja['faces']=[f for f in hoja['faces'] if all(hoja['vertices'][i][0]<-445000 for i in f)]
    export([hoja],ROOT/'scalpel-google/hoja.obj',transform=lambda v:((v[0]+500000)*.000001,(v[1]-392971)*.000001,(v[2]-250000)*.000001))
    ps=piezas('display-mannequin/mannequin.glb')
    for i,p in enumerate(ps[:9]):
        def pose(v):
            x,y,z=v
            if i<=4:
                a=math.radians(-75);dy=y-.48
                return x,(.86+dy*math.cos(a)-z*math.sin(a)) if i<3 else .88+(y-.48)*.18,.55+dy*math.sin(a)+z*math.cos(a)
            if y<.42:
                return x,.84+(z-.42)*.5,.55+.42*1.4+(.42-y)*1.4
            return x,y+.4,z*1.4+.55
        # Exporta por pieza para poder asignar material de piel o uniforme.
        export([p],ROOT/('display-mannequin/paciente-'+str(i)+'.obj'),transform=pose)

if __name__=='__main__' and len(__import__('sys').argv)==1:preparar()

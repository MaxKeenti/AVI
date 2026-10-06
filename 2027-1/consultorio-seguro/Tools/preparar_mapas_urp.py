"""Empaqueta canales PBR CC0 para URP sin alterar los archivos originales."""
import hashlib, json, pathlib
from PIL import Image, ImageChops
RAIZ=pathlib.Path(__file__).resolve().parents[1]/'Assets'/'Terceros'
NOTA='Derivación técnica de mapas URP: metallic_gloss PNG RGBA con R=metálico, A=1-rugosidad; occlusion PNG con G=oclusión ambiental. El follaje combina color y máscara alfa original en PNG RGBA. Los originales se conservan sin cambios. Importar estos mapas con sRGB desactivado.'

def guardar(imagen,ruta):
    imagen.save(ruta,compress_level=6)
    return {'archivo':str(ruta.relative_to(RAIZ)),'sha256':hashlib.sha256(ruta.read_bytes()).hexdigest(),'tipo':'derivado_urp'}

for carpeta in sorted(RAIZ.glob('polyhaven-*')):
    mp=carpeta/'materiales.json'
    if mp.exists():materiales=json.loads(mp.read_text())
    else:
        nombre=carpeta.name.removeprefix('polyhaven-').replace('-','_')
        materiales=[{'nombre':nombre,'base_color':nombre+'_diff_2k.jpg','normal_gl':nombre+'_nor_gl_2k.jpg','roughness_o_arm':nombre+'_rough_2k.jpg','metallic':0,'doble_cara':False,'alpha_mode':'OPAQUE'}]
    derivados=[]
    for m in materiales:
        ruta=carpeta/m['roughness_o_arm'];original=Image.open(ruta).convert('RGB');r,g,b=original.split();cero=Image.new('L',original.size,0);blanco=Image.new('L',original.size,255)
        combinado='_arm_' in ruta.name
        metal=b if combinado else cero
        aspereza=g if combinado else r
        ao=r if combinado else blanco
        ruta_ao=ruta.with_name(m['nombre']+'_ao_2k.jpg')
        if ruta_ao.exists():ao=Image.open(ruta_ao).convert('L')
        # URP: rugosidad perceptual invertida en alfa; verde para oclusión.
        mg=Image.merge('RGBA',(metal,cero,cero,ImageChops.invert(aspereza)))
        occlusion=Image.merge('RGB',(blanco,ao,blanco))
        destino=ruta.with_name(m['nombre']+'_metallic_gloss_2k.png');derivados.append(guardar(mg,destino));m['metallic_gloss']=str(destino.relative_to(carpeta))
        destino=ruta.with_name(m['nombre']+'_occlusion_2k.png');derivados.append(guardar(occlusion,destino));m['occlusion']=str(destino.relative_to(carpeta))
        ruta_alpha=ruta.with_name(m['nombre']+'_alpha_2k.jpg')
        if ruta_alpha.exists():
            original_color=m.get('base_color_original',m['base_color']);color=Image.open(carpeta/original_color).convert('RGB');alpha=Image.open(ruta_alpha).convert('L')
            destino=ruta.with_name(m['nombre']+'_base_alpha_2k.png');derivados.append(guardar(Image.merge('RGBA',(*color.split(),alpha)),destino))
            m['base_color_original']=original_color;m['base_color']=str(destino.relative_to(carpeta));m['alpha_cutoff']=0.5
        m['metallic_gloss_srgb']=False;m['occlusion_srgb']=False;m['normal_import_type']='NormalMap';m['normal_encoding']='OpenGL';m['smoothness_texture_channel']='alfa';m['occlusion_texture_channel']='verde'
    mp.write_text(json.dumps(materiales,ensure_ascii=False,indent=2)+'\n')
    procedencia=carpeta/'procedencia.json';p=json.loads(procedencia.read_text());p['derivados']=derivados
    if NOTA not in p['modificaciones']:p['modificaciones']+=' '+NOTA
    procedencia.write_text(json.dumps(p,ensure_ascii=False,indent=2)+'\n')
    f=carpeta/'FUENTE.md';s=f.read_text()
    if NOTA not in s:f.write_text(s+'\n'+NOTA+'\n')
    print(carpeta.name,len(derivados),'mapas URP',flush=True)

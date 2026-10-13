"""Descarga recursos CC0 y convierte glTF a OBJ conservando normales y UV.
Los materiales PBR originales quedan documentados en materiales.json.
"""
import concurrent.futures, hashlib, json, pathlib, struct, urllib.request

RAIZ = pathlib.Path(__file__).resolve().parents[1] / 'Assets' / 'Terceros'
CDN = 'https://dl.polyhaven.org/file/ph-assets/'
MODELOS = {
    'modern_arm_chair_01': ('Modern Arm Chair 01', 'Vibrant Nordic'),
    'potted_plant_02': ('Potted Plant 02', 'Rico Cilliers'),
}
TEXTURAS = {
    'oak_veneer_01': ('Oak Veneer 01', 'Jenelle van Heerden'),
    'beige_wall_001': ('Beige Wall 001', 'Dimitrios Savva; Rico Cilliers'),
}


def bajar(url, destino):
    destino.parent.mkdir(parents=True, exist_ok=True)
    if not destino.exists():
        with urllib.request.urlopen(url, timeout=30) as respuesta:
            destino.write_bytes(respuesta.read())
    return {'archivo': str(destino.relative_to(RAIZ)), 'url': url,
            'sha256': hashlib.sha256(destino.read_bytes()).hexdigest()}


def documentar(carpeta, identificador, titulo, autor, descargas, modificaciones):
    datos = {'titulo': titulo, 'autor': autor, 'url': 'https://polyhaven.com/a/' + identificador,
             'licencia': 'CC0 1.0 Universal', 'licencia_url': 'https://polyhaven.com/license',
             'licencia_texto': 'https://creativecommons.org/publicdomain/zero/1.0/',
             'consulta': '2026-10-02', 'modificaciones': modificaciones, 'descargas': descargas}
    (carpeta/'procedencia.json').write_text(json.dumps(datos, ensure_ascii=False, indent=2)+'\n')
    (carpeta/'FUENTE.md').write_text(f'# {titulo}\n\nAutor: {autor}.\n\nFuente: {datos["url"]}\n\nLicencia: CC0 1.0 Universal. Ver https://polyhaven.com/license y https://creativecommons.org/publicdomain/zero/1.0/.\n\nConsulta: 2026-10-02.\n\nModificaciones: {modificaciones}\n\nLos archivos originales, URL de descarga y sumas SHA-256 se conservan en `procedencia.json`.\n')


def modelo(identificador, titulo, autor):
    carpeta = RAIZ/('polyhaven-'+identificador.replace('_','-'))
    base = CDN+f'Models/gltf/2k/{identificador}/'
    descargas = [bajar(base+identificador+'_2k.gltf', carpeta/(identificador+'_2k.gltf'))]
    gltf = json.loads((carpeta/(identificador+'_2k.gltf')).read_text())
    solicitudes = [(base+b['uri'], carpeta/b['uri']) for b in gltf['buffers']]
    solicitudes += [(CDN+f'Models/jpg/2k/{identificador}/'+pathlib.Path(i['uri']).name, carpeta/i['uri']) for i in gltf['images']]
    if identificador == 'potted_plant_02':
        solicitudes += [(CDN+f'Models/jpg/2k/{identificador}/{identificador}_{mapa}_2k.jpg', carpeta/'textures'/f'{identificador}_{mapa}_2k.jpg') for mapa in ['leaves_alpha','leaves_ao','pot_ao']]
    with concurrent.futures.ThreadPoolExecutor(max_workers=4) as ejecutor:
        descargas += list(ejecutor.map(lambda x:bajar(*x), solicitudes))
    buffers = [(carpeta/b['uri']).read_bytes() for b in gltf['buffers']]
    def leer(indice):
        a=gltf['accessors'][indice]; vista=gltf['bufferViews'][a['bufferView']]
        formato, tamano={5121:('B',1),5123:('H',2),5125:('I',4),5126:('f',4)}[a['componentType']]
        n={'SCALAR':1,'VEC2':2,'VEC3':3,'VEC4':4}[a['type']]
        inicio=vista.get('byteOffset',0)+a.get('byteOffset',0); salto=vista.get('byteStride',tamano*n)
        return [struct.unpack_from('<'+formato*n,buffers[vista.get('buffer',0)],inicio+i*salto) for i in range(a['count'])]
    obj=['# Recurso CC0 de Poly Haven; geometría original, normales y UV preservadas.', f'mtllib {identificador}.mtl']
    offset=1; vertices=[]; triangulos=0
    for nodo in gltf['nodes']:
        if 'mesh' not in nodo: continue
        assert 'rotation' not in nodo and 'scale' not in nodo and 'matrix' not in nodo
        traslacion=nodo.get('translation',[0,0,0])
        for primitiva in gltf['meshes'][nodo['mesh']]['primitives']:
            atributos=primitiva['attributes']; ps=leer(atributos['POSITION']); ns=leer(atributos['NORMAL']); uv=leer(atributos['TEXCOORD_0']); ids=leer(primitiva['indices'])
            obj += ['o '+nodo['name'], 'usemtl '+gltf['materials'][primitiva['material']]['name']]
            for p in ps:
                p=tuple(p[i]+traslacion[i] for i in range(3));vertices.append(p);obj.append('v '+' '.join(f'{x:.7f}' for x in p))
            obj += ['vt '+f'{p[0]:.7f} {1-p[1]:.7f}' for p in uv]
            obj += ['vn '+' '.join(f'{x:.7f}' for x in p) for p in ns]
            for k in range(0,len(ids),3):
                obj.append('f '+' '.join(f'{ids[k+i][0]+offset}/{ids[k+i][0]+offset}/{ids[k+i][0]+offset}' for i in range(3)))
                triangulos += 1
            offset += len(ps)
    (carpeta/(identificador+'.obj')).write_text('\n'.join(obj)+'\n')
    mtl=[];materiales=[]
    def textura(indice):return gltf['images'][gltf['textures'][indice]['source']]['uri']
    for material in gltf['materials']:
        pbr=material['pbrMetallicRoughness'];dif=textura(pbr['baseColorTexture']['index']);normal=textura(material['normalTexture']['index']);arm=textura(pbr['metallicRoughnessTexture']['index'])
        mtl += ['newmtl '+material['name'],'Kd 1 1 1','Ks 0.04 0.04 0.04','Ns 40','map_Kd '+dif,'map_Bump '+normal]
        materiales.append({'nombre':material['name'],'base_color':dif,'normal_gl':normal,'roughness_o_arm':arm,'metallic':pbr.get('metallicFactor',1),'doble_cara':material.get('doubleSided',False),'alpha_mode':material.get('alphaMode','OPAQUE')})
    (carpeta/(identificador+'.mtl')).write_text('\n'.join(mtl)+'\n')
    limites={'min':[min(p[i] for p in vertices) for i in range(3)],'max':[max(p[i] for p in vertices) for i in range(3)],'triangulos':triangulos,'unidad':'metro','eje_vertical':'+Y','notas':'OBJ conserva coordenadas glTF; UV V invertida para Wavefront. Comprobar orientación frontal en Unity.'}
    (carpeta/'materiales.json').write_text(json.dumps(materiales,indent=2,ensure_ascii=False)+'\n')
    (carpeta/'limites.json').write_text(json.dumps(limites,indent=2,ensure_ascii=False)+'\n')
    documentar(carpeta,identificador,titulo,autor,descargas,'Descarga de glTF y mapas PBR de 2K. Conversión a OBJ con normales, UV, grupos de materiales y traslaciones originales; inversión de V para Wavefront. Sin simplificación de malla. Escala y materiales finales configurados en Unity.')
    print(identificador, json.dumps(limites),flush=True)


def textura_pbr(identificador, titulo, autor):
    carpeta=RAIZ/('polyhaven-'+identificador.replace('_','-'))
    solicitudes=[(CDN+f'Textures/jpg/2k/{identificador}/{identificador}_{tipo}_2k.jpg',carpeta/f'{identificador}_{tipo}_2k.jpg') for tipo in ['diff','nor_gl','rough','ao']]
    with concurrent.futures.ThreadPoolExecutor(max_workers=3) as ejecutor:descargas=list(ejecutor.map(lambda x:bajar(*x),solicitudes))
    documentar(carpeta,identificador,titulo,autor,descargas,'Selección de mapas de color base, normal OpenGL y rugosidad a 2K; repetición, intensidad y tinte configurados en Unity. Sin modificación de los originales.')
    print(identificador,'texturas listas',flush=True)


if __name__ == '__main__':
    for identificador, datos in MODELOS.items():modelo(identificador,*datos)
    for identificador, datos in TEXTURAS.items():textura_pbr(identificador,*datos)

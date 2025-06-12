# 🎨 Pixel Wall-E Paints  

**Un compilador para crear arte pixelado en una cuadrícula**  

![Demo](https://via.placeholder.com/800x400?text=Pixel+Wall-E+Paints+Demo) *(Reemplaza con una captura real de tu proyecto)*  

## 🔍 Descripción  
Pixel Wall-E Paints es un compilador diseñado para generar formas y dibujos en una cuadrícula estilo *pixel art*. Con un lenguaje de comandos sencillo, puedes crear diseños personalizados y exportarlos.  

## 🛠 Tecnologías  
- **Unity 6 (6000.0.32f1)** (Motor de juego y renderizado)  
- **C#** (Lógica del compilador y generación de gráficos)  

## ⚙️ Instalación  
### Opción 1: Clonar y abrir en Unity  
1. Clona este repositorio:  

   git clone https://github.com/GaboPS1016/Wall-E-Paints-Project.git
  
2. Abre el proyecto en **Unity Hub**.  
3. Ejecuta desde el editor o exporta el build.  

### Opción 2: Descargar ejecutable (para usuarios finales)  
- Descarga el archivo `.rar` desde [Releases](https://github.com/GaboPS1016/Wall-E-Paints-Project/releases) y descomprímelo.  
- Ejecuta `PixelWallEPaints.exe`.  

## 📖 Uso  
El compilador sigue reglas específicas para generar los dibujos. Consulta el [Manual de Instrucciones](docs/Manual.pdf) (PDF incluido en el repositorio) para aprender la sintaxis de comandos y ejemplos.  

### Ejemplo básico:  
*Para dibujar un círculo de radio 4, de color azul y grosor 3:*

 Spawn(2,2)
 Color("Blue")
 Size(3)
 DrawCircle(1,1,4)

## 🎯 Características  
- Lenguaje de comandos intuitivo.  
- Guardado y cargado de archivos con extensión .pw  
- Programa de *pixel art*.  

## 🤝 Contribución  
¿Quieres mejorar Pixel Wall-E Paints?  
1. Haz un *fork* del repositorio.  
2. Crea una rama con tu feature: `git checkout -b mi-feature`.  
3. Envía un *Pull Request*.  

## 📜 Licencia  
MIT License. Ver [LICENSE](LICENSE) para más detalles.  
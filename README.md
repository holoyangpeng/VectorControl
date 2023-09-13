# VectorControl

VectorControl是一个C#开发的矢量图形开发组件，实现了目前主流的矢量图形编辑软件所提供的的大部分功能，可用于组态、建模、工控、仿真以及其他需要使用图形渲染和编辑的开发场景。
以SVG文件作为底层图形存储格式，可以重用市面上主流图形软件生成的SVG文件进行开发。  
VectorControl第一版大概写于05年左右，当时以闭源的形式提供，约13，14年左右停止更新维护。但想着或许可作为图形开发中的他山之石，遂开源。  
相关问题建议，可以联系：yypprr@sohu.com

开发环境：VS2015以上  
OS：Windows（理论上.Net通过Mono可应用在其他非Windows环境中，但目前代码中包含部分Win32引用，暂时不能无缝迁移）  

代码以标准的VS方案提供，包含如下项目：  
VectorControl  
├── YP.CommonControl          //实现了一些通用的UI布局组件  
├── YP.CSS                    //一个简单的CSS解析库，用于解析SVG文件中的CSS定义  
├── YP.SVG                    //SVG解析和渲染引擎，基于SVG1.1（非完全实现），开发中参考了开源项目SVG#  
├── YP.VectorControl          //具体实现图形渲染、编辑逻辑，并通过.Net标准组件的形式提供一个Canvas组件类，可以直接嵌入Form界面中进行开发  
├── YP.SymbolDesigner         //图元设计器，同时也是使用Canvas组件二次开发的实例，通过Canvas组件实现了一个标准的矢量图形编辑环境  
  
运行程序可以打开解决方案后编译运行SymbolDesigner项目，也可以直接从Program文件夹中下载编译好的程序直接运行。SymbolDesigner提供了几个简单实例，分别展示了如何使用VectorContol开发电路、电网、流程图、组态以及地图等。  
  
![demo1](https://github.com/holoyangpeng/VectorControl/assets/114057336/1d549afc-802f-43b6-9db2-38aa31546fc5)
![demo2](https://github.com/holoyangpeng/VectorControl/assets/114057336/8951da6b-6849-47e7-9515-a1e5a1c74879)
![demo3](https://github.com/holoyangpeng/VectorControl/assets/114057336/73c1a1c0-a733-4858-89e9-b05bfd03e5d1)
![demo4](https://github.com/holoyangpeng/VectorControl/assets/114057336/4430a63e-0cff-494d-9bad-9b5d96a41014)
![demo5](https://github.com/holoyangpeng/VectorControl/assets/114057336/d77a70f7-18d4-4ad1-b4c8-d960a3723925)
![demo6](https://github.com/holoyangpeng/VectorControl/assets/114057336/9c892625-1e1b-4117-858d-1fe5152d0739)


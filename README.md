![demo3](https://github.com/holoyangpeng/VectorControl/assets/114057336/f8665675-0361-4601-8a48-477984dc473e)# VectorControl

VectorControl是一个C#开发的矢量图形开发组件，实现了目前主流的矢量图形编辑软件所提供的的大部分功能，可用于组态、建模、工控、仿真以及其他需要使用图形渲染和编辑的开发场景。以SVG文件作为底层格式，可以重用市面上主流图形软件生成的SVG文件进行开发。  
VectorControl第一版大概写于05年左右，当时以闭源的形式提供，最后一次更新大约在14年左右。但想着或许可作为图形开发中的他山之石，遂开源。  
相关问题建议，可以联系：yypprr@sohu.com

快速浏览功能可直接下载Program文件夹中编译好的程序直接运行

开发环境：VS2015以上  
OS：Windows（理论上.Net通过Mono可应用在其他非Windows环境中，但目前代码中包含部分Win32引用，暂时不能直接迁移）  

代码以标准的VS方案提供，包含如下项目：  
VectorControl  
├── YP.CommonControl          //实现了一些通用的UI布局组件  
├── YP.CSS                    //一个简单的CSS解析库，用于解析SVG文件中的CSS定义  
├── YP.SVG                    //SVG解析和渲染引擎，基于SVG1.1（非完全实现），开发中参考了开源项目SVG#  
├── YP.VectorControl          //具体实现图形渲染、编辑逻辑，并通过.Net标准组件的形式提供一个Canvas组件类，可以直接嵌入Form界面中进行开发  
├── YP.SymbolDesigner         //图元设计器，同时也是使用Canvas组件二次开发的实例，通过Canvas组件实现了一个标准的矢量图形编辑环境  
  
运行程序可以打开解决方案后编译运行SymbolDesigner项目。SymbolDesigner提供了几个简单实例，分别展示了如何使用VectorContol开发电路、电网、流程图、组态以及地图等。  
![demo1](https://github.com/holoyangpeng/VectorControl/assets/114057336/9f3c911f-d9e6-4d5f-b898-768caf168427)
![demo2](https://github.com/holoyangpeng/VectorControl/assets/114057336/5787d635-e4e6-4d00-bbd6-0392e1034670)
!![demo4](https://github.com/holoyangpeng/VectorControl/assets/114057336/0755b73e-d8d9-4290-92ae-3f7b52841d2f)
![demo3](https://github.com/holoyangpeng/VectorControl/assets/114057336/240da646-26dd-4df6-b095-0a7a2432240c)
![demo5](https://github.com/holoyangpeng/VectorControl/assets/114057336/2db7c683-ca04-4f51-a1da-c0c897207db0)
![demo6](https://github.com/holoyangpeng/VectorControl/assets/114057336/5ef6d04e-a508-4ee0-9efa-d5ddf7f3f9ab)


### 20220421
- 下发调度规则，默认不准通行的人员将从相机人脸库中被删除

### 版本历史

- 离线设备下发失败，下发状态置为失败，避免出现下发进度无法推进的情况
- 修改文案为组织机构和人员类别

#### 2.10.9.0
- 人像下发线程采用ManuResetEvent信号唤醒，减少线程调用
- 修改多线程访问Deviceinfo.MyDevicelist可能引起的bug
- 增加是否保存实时数据到数据库开关选项(用户不可见)，缺省关闭
- 中文增加显示版本历史菜单：系统设置->关于


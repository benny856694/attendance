### 2.10.9.3
修复：
- 导入人员的时候trim空格
- 抓拍记录及时存入数据库，而不是全部保存在内存中
- 抓拍记录获取不完整, time+SN可能导致判断重复错误，该用sequence+SN判断
- 退出程序时，通知并等待工作线程完成工作
- 增加考勤计算开关
- 程序收到退出命令时，在系统托盘显示提示在后台完成计算
- Http获取抓拍记录，发送用户名和密码
- 修复长人名无法显示问题
- MyDevice表中的Last_query为0001-01-01时，程序逻辑异常
- 程序退出慢的问题

### 2.10.9.2
- 人员管理，增加是否有头像查询

### 2.10.9.1
- 下发调度规则，默认不准通行的人员将从相机人脸库中被删除
- 离线设备下发失败，下发状态置为失败，避免出现下发进度无法推进的情况
- 修改文案为组织机构和人员类别
- 优化设备参数设置，判断参数中字段可能不存在的情况
- 编辑人员，先判断是否自动下发，才加入到下发列表

#### 2.10.9.0
- 人像下发线程采用ManuResetEvent信号唤醒，减少线程调用
- 修改多线程访问Deviceinfo.MyDevicelist可能引起的bug
- 增加是否保存实时数据到数据库开关选项(用户不可见)，缺省关闭
- 中文增加显示版本历史菜单：系统设置->关于


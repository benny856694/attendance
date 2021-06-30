﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace huaanClient.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("huaanClient.Properties.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 旷工.
        /// </summary>
        internal static string Absent {
            get {
                return ResourceManager.GetString("Absent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 没有可用的磁盘，无法保存数据.
        /// </summary>
        internal static string AllDriveIsNotAvailable {
            get {
                return ResourceManager.GetString("AllDriveIsNotAvailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to name,department,Employee_code,Date,Punchinformation,Punchinformation1,Shiftinformation,Duration,late,Leaveearly,workOvertime,isAbsenteeism,temperature.
        /// </summary>
        internal static string AttendanceKeys {
            get {
                return ResourceManager.GetString("AttendanceKeys", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 姓名,部门,员工编号,考勤日期,上班打卡,下班打卡,班次信息,应出勤时间(小时),迟到(分钟),早退(分钟),加班(分钟),是否旷工,体温(℃).
        /// </summary>
        internal static string AttendanceNames {
            get {
                return ResourceManager.GetString("AttendanceNames", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 相机.
        /// </summary>
        internal static string Camera {
            get {
                return ResourceManager.GetString("Camera", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 请假.
        /// </summary>
        internal static string DayOff {
            get {
                return ResourceManager.GetString("DayOff", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 图片缺失.
        /// </summary>
        internal static string ImageMissing {
            get {
                return ResourceManager.GetString("ImageMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 正在切换....
        /// </summary>
        internal static string PromptSwitchingCamera {
            get {
                return ResourceManager.GetString("PromptSwitchingCamera", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 保存失败.
        /// </summary>
        internal static string SaveFailed {
            get {
                return ResourceManager.GetString("SaveFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 保存成功.
        /// </summary>
        internal static string SaveSuccess {
            get {
                return ResourceManager.GetString("SaveSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 员工编号已经存在.
        /// </summary>
        internal static string StaffCodeExists {
            get {
                return ResourceManager.GetString("StaffCodeExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 员工编号不能为空.
        /// </summary>
        internal static string StaffCodeIsEmpty {
            get {
                return ResourceManager.GetString("StaffCodeIsEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 照片不合符要求.
        /// </summary>
        internal static string StaffImageInValid {
            get {
                return ResourceManager.GetString("StaffImageInValid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 切换.
        /// </summary>
        internal static string Switch {
            get {
                return ResourceManager.GetString("Switch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 拍照.
        /// </summary>
        internal static string TakePicture {
            get {
                return ResourceManager.GetString("TakePicture", resourceCulture);
            }
        }
    }
}

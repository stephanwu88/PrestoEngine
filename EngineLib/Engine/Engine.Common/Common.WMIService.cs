using System;
using System.Management;
using System.Collections.Generic;

namespace Engine.Common
{
    /// <summary>
    /// WMI类名称枚举
    /// </summary>
    public enum WmiClass
    {
        Win32_1394Controller,                   
        Win32_1394ControllerDevice,             
        Win32_Account,
        Win32_AccountSID,
        Win32_ACE,
        Win32_ActionCheck,
        Win32_AllocatedResource,
        Win32_ApplicationCommandLine,
        Win32_ApplicationService,
        Win32_AssociatedBattery,
        Win32_AssociatedProcessorMemory,
        Win32_BaseBoard,
        Win32_BaseService,
        Win32_Battery,
        Win32_Binary,                       
        Win32_BindImageAction,
        Win32_BIOS,
        Win32_BootConfiguration,
        Win32_Bus,
        Win32_CacheMemory,
        Win32_CDROMDrive,
        Win32_CheckCheck,
        Win32_CIMLogicalDeviceCIMDataFile,
        Win32_ClassicCOMApplicationClasses,
        Win32_ClassicCOMClass,
        Win32_ClassicCOMClassSetting,
        Win32_ClassicCOMClassSettings,
        Win32_ClassInfoAction,
        Win32_ClientApplicationSetting,
        Win32_CodecFile,
        Win32_COMApplication,
        Win32_COMApplicationClasses,
        Win32_COMApplicationSettings,
        Win32_COMClass,
        Win32_ComClassAutoEmulator,
        Win32_ComClassEmulator,
        Win32_CommandLineAccess,
        Win32_ComponentCategory,
        Win32_ComputerSystem,
        Win32_ComputerSystemProcessor,
        Win32_ComputerSystemProduct,
        Win32_COMSetting,
        Win32_Condition,
        Win32_CreateFolderAction,
        Win32_CurrentProbe,
        Win32_DCOMApplication,
        Win32_DCOMApplicationAccessAllowedSetting,
        Win32_DCOMApplicationLaunchAllowedSetting,
        Win32_DCOMApplicationSetting,
        Win32_DependentService,
        Win32_Desktop,
        Win32_DesktopMonitor,
        Win32_DeviceBus,
        Win32_DeviceMemoryAddress,
        Win32_DeviceSettings,
        Win32_Directory,
        Win32_DirectorySpecification,
        Win32_DiskDrive,
        Win32_DiskDriveToDiskPartition,
        Win32_DiskPartition,
        Win32_DisplayConfiguration,
        Win32_DisplayControllerConfiguration,
        Win32_DMAChannel,
        Win32_DriverVXD,
        Win32_DuplicateFileAction,
        Win32_Environment,
        Win32_EnvironmentSpecification,
        Win32_ExtensionInfoAction,
        Win32_Fan,
        Win32_FileSpecification,
        Win32_FloppyController,
        Win32_FloppyDrive,
        Win32_FontInfoAction,
        Win32_Group,
        Win32_GroupUser,
        Win32_HeatPipe,
        Win32_IDEController,
        Win32_IDEControllerDevice,
        Win32_ImplementedCategory,
        Win32_InfraredDevice,
        Win32_IniFileSpecification,
        Win32_InstalledSoftwareElement,
        Win32_IRQResource,
        Win32_Keyboard,
        Win32_LaunchCondition,
        Win32_LoadOrderGroup,
        Win32_LoadOrderGroupServiceDependencies,
        Win32_LoadOrderGroupServiceMembers,
        Win32_LogicalDisk,
        Win32_LogicalDiskRootDirectory,
        Win32_LogicalDiskToPartition,
        Win32_LogicalFileAccess,
        Win32_LogicalFileAuditing,
        Win32_LogicalFileGroup,
        Win32_LogicalFileOwner,
        Win32_LogicalFileSecuritySetting,
        Win32_LogicalMemoryConfiguration,
        Win32_LogicalProgramGroup,
        Win32_LogicalProgramGroupDirectory,
        Win32_LogicalProgramGroupItem,
        Win32_LogicalProgramGroupItemDataFile,
        Win32_LogicalShareAccess,
        Win32_LogicalShareAuditing,
        Win32_LogicalShareSecuritySetting,
        Win32_ManagedSystemElementResource,
        Win32_MemoryArray,
        Win32_MemoryArrayLocation,
        Win32_MemoryDevice,
        Win32_MemoryDeviceArray,
        Win32_MemoryDeviceLocation,
        Win32_MethodParameterClass,
        Win32_MIMEInfoAction,
        Win32_MotherboardDevice,
        Win32_MoveFileAction,
        Win32_MSIResource,
        Win32_NetworkAdapter,
        Win32_NetworkAdapterConfiguration,
        Win32_NetworkAdapterSetting,
        Win32_NetworkClient,
        Win32_NetworkConnection,
        Win32_NetworkLoginProfile,
        Win32_NetworkProtocol,
        Win32_NTEventlogFile,
        Win32_NTLogEvent,
        Win32_NTLogEventComputer,
        Win32_NTLogEventLog,
        Win32_NTLogEventUser,
        Win32_ODBCAttribute,
        Win32_ODBCDataSourceAttribute,
        Win32_ODBCDataSourceSpecification,
        Win32_ODBCDriverAttribute,
        Win32_ODBCDriverSoftwareElement,
        Win32_ODBCDriverSpecification,
        Win32_ODBCSourceAttribute,
        Win32_ODBCTranslatorSpecification,
        Win32_OnBoardDevice,
        Win32_OperatingSystem,
        Win32_OperatingSystemQFE,
        Win32_OSRecoveryConfiguration,
        Win32_PageFile,
        Win32_PageFileElementSetting,
        Win32_PageFileSetting,
        Win32_PageFileUsage,
        Win32_ParallelPort,
        Win32_Patch,
        Win32_PatchFile,
        Win32_PatchPackage,
        Win32_PCMCIAController,
        Win32_Perf,
        Win32_PerfRawData,
        Win32_PerfRawData_ASP_ActiveServerPages,
        Win32_PerfRawData_ASPNET_114322_ASPNETAppsv114322,
        Win32_PerfRawData_ASPNET_114322_ASPNETv114322,
        Win32_PerfRawData_ASPNET_ASPNET,
        Win32_PerfRawData_ASPNET_ASPNETApplications,
        Win32_PerfRawData_IAS_IASAccountingClients,
        Win32_PerfRawData_IAS_IASAccountingServer,
        Win32_PerfRawData_IAS_IASAuthenticationClients,
        Win32_PerfRawData_IAS_IASAuthenticationServer,
        Win32_PerfRawData_InetInfo_InternetInformationServicesGlobal,
        Win32_PerfRawData_MSDTC_DistributedTransactionCoordinator,
        Win32_PerfRawData_MSFTPSVC_FTPService,
        Win32_PerfRawData_MSSQLSERVER_SQLServerAccessMethods,
        Win32_PerfRawData_MSSQLSERVER_SQLServerBackupDevice,
        Win32_PerfRawData_MSSQLSERVER_SQLServerBufferManager,
        Win32_PerfRawData_MSSQLSERVER_SQLServerBufferPartition,
        Win32_PerfRawData_MSSQLSERVER_SQLServerCacheManager,
        Win32_PerfRawData_MSSQLSERVER_SQLServerDatabases,
        Win32_PerfRawData_MSSQLSERVER_SQLServerGeneralStatistics,
        Win32_PerfRawData_MSSQLSERVER_SQLServerLatches,
        Win32_PerfRawData_MSSQLSERVER_SQLServerLocks,
        Win32_PerfRawData_MSSQLSERVER_SQLServerMemoryManager,
        Win32_PerfRawData_MSSQLSERVER_SQLServerReplicationAgents,
        Win32_PerfRawData_MSSQLSERVER_SQLServerReplicationDist,
        Win32_PerfRawData_MSSQLSERVER_SQLServerReplicationLogreader,
        Win32_PerfRawData_MSSQLSERVER_SQLServerReplicationMerge,
        Win32_PerfRawData_MSSQLSERVER_SQLServerReplicationSnapshot,
        Win32_PerfRawData_MSSQLSERVER_SQLServerSQLStatistics,
        Win32_PerfRawData_MSSQLSERVER_SQLServerUserSettable,
        Win32_PerfRawData_NETFramework_NETCLRExceptions,
        Win32_PerfRawData_NETFramework_NETCLRInterop,
        Win32_PerfRawData_NETFramework_NETCLRJit,
        Win32_PerfRawData_NETFramework_NETCLRLoading,
        Win32_PerfRawData_NETFramework_NETCLRLocksAndThreads,
        Win32_PerfRawData_NETFramework_NETCLRMemory,
        Win32_PerfRawData_NETFramework_NETCLRRemoting,
        Win32_PerfRawData_NETFramework_NETCLRSecurity,
        Win32_PerfRawData_Outlook_Outlook,
        Win32_PerfRawData_PerfDisk_PhysicalDisk,
        Win32_PerfRawData_PerfNet_Browser,
        Win32_PerfRawData_PerfNet_Redirector,
        Win32_PerfRawData_PerfNet_Server,
        Win32_PerfRawData_PerfNet_ServerWorkQueues,
        Win32_PerfRawData_PerfOS_Cache,
        Win32_PerfRawData_PerfOS_Memory,
        Win32_PerfRawData_PerfOS_Objects,
        Win32_PerfRawData_PerfOS_PagingFile,
        Win32_PerfRawData_PerfOS_Processor,
        Win32_PerfRawData_PerfOS_System,
        Win32_PerfRawData_PerfProc_FullImage_Costly,
        Win32_PerfRawData_PerfProc_Image_Costly,
        Win32_PerfRawData_PerfProc_JobObject,
        Win32_PerfRawData_PerfProc_JobObjectDetails,
        Win32_PerfRawData_PerfProc_Process,
        Win32_PerfRawData_PerfProc_ProcessAddressSpace_Costly,
        Win32_PerfRawData_PerfProc_Thread,
        Win32_PerfRawData_PerfProc_ThreadDetails_Costly,
        Win32_PerfRawData_RemoteAccess_RASPort,
        Win32_PerfRawData_RemoteAccess_RASTotal,
        Win32_PerfRawData_RSVP_ACSPerRSVPService,
        Win32_PerfRawData_Spooler_PrintQueue,
        Win32_PerfRawData_TapiSrv_Telephony,
        Win32_PerfRawData_Tcpip_ICMP,
        Win32_PerfRawData_Tcpip_IP,
        Win32_PerfRawData_Tcpip_NBTConnection,
        Win32_PerfRawData_Tcpip_NetworkInterface,
        Win32_PerfRawData_Tcpip_TCP,
        Win32_PerfRawData_Tcpip_UDP,
        Win32_PerfRawData_W3SVC_WebService,
        Win32_PhysicalMemory,
        Win32_PhysicalMemoryArray,
        Win32_PhysicalMemoryLocation,
        Win32_PNPAllocatedResource,
        Win32_PnPDevice,
        Win32_PnPEntity,
        Win32_PointingDevice,
        Win32_PortableBattery,
        Win32_PortConnector,
        Win32_PortResource,
        Win32_POTSModem,
        Win32_POTSModemToSerialPort,
        Win32_PowerManagementEvent,
        Win32_Printer,
        Win32_PrinterConfiguration,
        Win32_PrinterController,
        Win32_PrinterDriverDll,
        Win32_PrinterSetting,
        Win32_PrinterShare,
        Win32_PrintJob,
        Win32_PrivilegesStatus,
        Win32_Process,
        Win32_Processor,
        Win32_ProcessStartup,
        Win32_Product,
        Win32_ProductCheck,
        Win32_ProductResource,
        Win32_ProductSoftwareFeatures,
        Win32_ProgIDSpecification,
        Win32_ProgramGroup,
        Win32_ProgramGroupContents,
        Win32_ProgramGroupOrItem,
        Win32_Property,
        Win32_ProtocolBinding,
        Win32_PublishComponentAction,
        Win32_QuickFixEngineering,
        Win32_Refrigeration,
        Win32_Registry,
        Win32_RegistryAction,
        Win32_RemoveFileAction,
        Win32_RemoveIniAction,
        Win32_ReserveCost,
        Win32_ScheduledJob,
        Win32_SCSIController,
        Win32_SCSIControllerDevice,
        Win32_SecurityDescriptor,
        Win32_SecuritySetting,
        Win32_SecuritySettingAccess,
        Win32_SecuritySettingAuditing,
        Win32_SecuritySettingGroup,
        Win32_SecuritySettingOfLogicalFile,
        Win32_SecuritySettingOfLogicalShare,
        Win32_SecuritySettingOfObject,
        Win32_SecuritySettingOwner,
        Win32_SelfRegModuleAction,
        Win32_SerialPort,
        Win32_SerialPortConfiguration,
        Win32_SerialPortSetting,
        Win32_Service,
        Win32_ServiceControl,
        Win32_ServiceSpecification,
        Win32_ServiceSpecificationService,
        Win32_SettingCheck,
        Win32_Share,
        Win32_ShareToDirectory,
        Win32_ShortcutAction,
        Win32_ShortcutFile,
        Win32_ShortcutSAP,
        Win32_SID,
        Win32_SMBIOSMemory,
        Win32_SoftwareElement,
        Win32_SoftwareElementAction,
        Win32_SoftwareElementCheck,
        Win32_SoftwareElementCondition,
        Win32_SoftwareElementResource,
        Win32_SoftwareFeature,
        Win32_SoftwareFeatureAction,
        Win32_SoftwareFeatureCheck,
        Win32_SoftwareFeatureParent,
        Win32_SoftwareFeatureSoftwareElements,
        Win32_SoundDevice,
        Win32_StartupCommand,
        Win32_SubDirectory,
        Win32_SystemAccount,
        Win32_SystemBIOS,
        Win32_SystemBootConfiguration,
        Win32_SystemDesktop,
        Win32_SystemDevices,
        Win32_SystemDriver,
        Win32_SystemDriverPNPEntity,
        Win32_SystemEnclosure,
        Win32_SystemLoadOrderGroups,
        Win32_SystemLogicalMemoryConfiguration,
        Win32_SystemMemoryResource,
        Win32_SystemNetworkConnections,
        Win32_SystemOperatingSystem,
        Win32_SystemPartitions,
        Win32_SystemProcesses,
        Win32_SystemProgramGroups,
        Win32_SystemResources,
        Win32_SystemServices,
        Win32_SystemSetting,
        Win32_SystemSlot,
        Win32_SystemSystemDriver,
        Win32_SystemTimeZone,
        Win32_SystemUsers,
        Win32_TapeDrive,
        Win32_TemperatureProbe,
        Win32_Thread,
        Win32_TimeZone,
        Win32_Trustee,
        Win32_TypeLibraryAction,
        Win32_UninterruptiblePowerSupply,
        Win32_USBController,
        Win32_USBControllerDevice,
        Win32_UserAccount,
        Win32_UserDesktop,
        Win32_VideoConfiguration,
        Win32_VideoController,
        Win32_VideoSettings,
        Win32_VoltageProbe,
        Win32_WMIElementSetting,
        Win32_WMISetting,
    }

    /// <summary>
    /// 磁盘驱动器信息
    /// </summary>
    public class DiskDriveItem
    {
        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 接口类型
        /// </summary>
        public string InterfaceType { get; set; }
        /// <summary>
        /// 序列号
        /// </summary>
        public string SerialNumber { get; set; }
    }

    /// <summary>
    /// WMI计算机管理工具操作
    /// Windows Management Instrumentation
    /// </summary>
    public partial class WmiService
    {
        private static WmiService _Default;

        /// <summary>
        /// 默认实例
        /// </summary>
        public static WmiService Default
        {
            get
            {
                if (_Default == null)
                    _Default = new WmiService();
                return _Default;
            }
        }

        /// <summary>
        /// 获取WMI实例
        /// </summary>
        /// <param name="ClassName">WMI类名</param>
        /// <param name="strWhere">查询条件</param>
        /// <returns></returns>
        public ManagementObjectCollection GetMOC(WmiClass ClassName, string strWhere)
        {
            string strClassName = Enum.GetName(typeof(WmiClass), ClassName);
            string strSqlWhere = string.Empty;
            if (strWhere.IsNotEmpty())
                strSqlWhere = $"where {strWhere.Replace("where","")}";
            string strQuery = $"select * from {strClassName} {strSqlWhere}";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(strQuery);
            //ManagementClass searcher = new ManagementClass(Key);
            //ManagementObjectCollection moc = searcher.GetInstances();
            ManagementObjectCollection moc = searcher.Get();
            return moc;
        }

        /// <summary>
        /// 获取WMI指定字段信息
        /// </summary>
        /// <param name="ClassName"></param>
        /// <param name="Item"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<string> GetMocItemList(WmiClass ClassName, string Item, string strWhere)
        {
            List<string> InstanceItemVal = new List<string>();
            try
            {
                ManagementObjectCollection moc = GetMOC(ClassName, strWhere);
                if (moc == null)
                    return InstanceItemVal;
                foreach (var mo in moc)
                {
                    //InstanceItemVal.Add(mo.Properties[Item].Value.ToMyString());
                    InstanceItemVal.Add(mo[Item].ToMyString());
                    mo.Dispose();
                }
                moc.Dispose();
            }
            catch (Exception ex) { }
            return InstanceItemVal;
        }
    }

    /// <summary>
    /// Wmi - 计算机主板、CPU、磁盘及外设
    /// </summary>
    public partial class WmiService
    {
        /// <summary>
        /// 主板序列号
        /// PF1KAMCP
        /// </summary>
        public string BoisKey
        {
            get
            {
                List<string> strGrpKey = GetMocItemList(WmiClass.Win32_BIOS, "SerialNumber", "");
                return strGrpKey.Count > 0 ? strGrpKey[0] : string.Empty;
            }
        }

        /// <summary>
        /// CPU处理器序列号
        /// BFEBFBFF000806EC
        /// </summary>
        public string ProcesserID
        {
            get
            {
                List<string> strGrpKey = GetMocItemList(WmiClass.Win32_Processor, "ProcessorId", "");
                return strGrpKey.Count > 0 ? strGrpKey[0] : string.Empty;
            }
        }

        /// <summary>
        /// 逻辑C盘序列号
        /// 5ADAE159
        /// </summary>
        public string SerialNumberLogicDiskC
        {
            get
            {
                string strWhere = "DeviceID = 'C:'";
                List<string> strGrpKey = GetMocItemList(WmiClass.Win32_LogicalDisk, "VolumeSerialNumber", strWhere);
                return strGrpKey.Count > 0 ? strGrpKey[0] : string.Empty;
            }
        }

        /// <summary>
        /// 计算机电池的设备ID
        /// 注意：台式机可能没有
        /// 3624SMP02DL008
        /// </summary>
        public string BatteryDeviceID
        {
            get
            {
                List<string> strGrpKey = GetMocItemList(WmiClass.Win32_Battery, "DeviceID", "");
                return strGrpKey.Count > 0 ? strGrpKey[0] : string.Empty;
            }
        }

        /// <summary>
        /// 计算机有效可访问的所有磁盘信息
        /// </summary>
        public List<DiskDriveItem> AccessiableDiskDriveItems
        {
            get => GetDiskDriveItems();
        }

        /// <summary>
        /// 计算机有效可访问的所有USB磁盘信息
        /// </summary>
        public List<DiskDriveItem> AccessiableUSBDiskDriveItems
        {
            get => GetDiskDriveItems("USB");
        }

        /// <summary>
        /// 获取可访问的磁盘集合
        /// </summary>
        /// <param name="InterfaceType"></param>
        /// <returns></returns>
        private List<DiskDriveItem> GetDiskDriveItems(string InterfaceType = "")
        {
            List<DiskDriveItem> DiskDriverItems = new List<DiskDriveItem>();
            //string strWhere = "Partitions > 0 and InterfaceType = 'USB' ";
            string strWhere = "Partitions > 0 ";
            if (!string.IsNullOrEmpty(InterfaceType))
                strWhere += string.Format(" and InterfaceType = '{0}' ", InterfaceType);
            ManagementObjectCollection moc = GetMOC(WmiClass.Win32_DiskDrive, strWhere);
            if (moc == null)
                return DiskDriverItems;
            foreach (var mo in moc)
            {
                DiskDriverItems.Add(new DiskDriveItem()
                {
                    Model = mo["Model"].ToMyString(),
                    InterfaceType = mo["InterfaceType"].ToMyString(),
                    SerialNumber = mo["SerialNumber"].ToMyString()
                });
                mo.Dispose();
            }
            moc.Dispose();
            return DiskDriverItems;
        }

        /// <summary>
        /// 目标磁盘查找确认
        /// </summary>
        /// <param name="item">磁盘相关信息</param>
        /// <returns></returns>
        public bool IsDiskMatched(DiskDriveItem item)
        {
            try
            {
                if (item == null)
                    return false;
                string strWhere = $"Partitions > 0 and Model = '{item.Model}' and InterfaceType ='{item.InterfaceType}' and SerialNumber = '{item.SerialNumber}'";
                ManagementObjectCollection moc = GetMOC(WmiClass.Win32_DiskDrive, strWhere);
                return moc.Count > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 目标主板查找确认
        /// </summary>
        /// <param name="serialNumber">主板序列号</param>
        /// <returns></returns>
        public bool IsBoisMatched(string serialNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(serialNumber))
                    return false;
                string strWhere = $"SerialNumber = '{serialNumber}'";
                ManagementObjectCollection moc = GetMOC(WmiClass.Win32_BIOS, strWhere);
                return moc.Count > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 目标处理器查找确认
        /// </summary>
        /// <param name="ProcessorId">处理器序列号</param>
        /// <returns></returns>
        public bool IsProcessorMatched(string ProcessorId)
        {
            try
            {
                if (string.IsNullOrEmpty(ProcessorId))
                    return false;
                string strWhere = $"ProcessorId = '{ProcessorId}'";
                ManagementObjectCollection moc = GetMOC(WmiClass.Win32_Processor, strWhere);
                return moc.Count > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Wmi - 计算机操作系统软信息 机器信息、系统及用户信息
    /// </summary>
    public partial class WmiService
    {
        /// <summary>
        /// 计算机服务编号（笔记背后标签、组装机没有）
        /// PF1KAMCP
        /// </summary>
        public string ComputerSysProductIdentifyNum
        {
            get
            {
                List<string> strGrpKey = GetMocItemList(WmiClass.Win32_ComputerSystemProduct, "IdentifyingNumber", "");
                return strGrpKey.Count > 0 ? strGrpKey[0] : string.Empty;
            }
        }

        /// <summary>
        /// 计算机系统产品UUID
        /// UUID是机器在工厂出货前烧录到DMI信息中的，重装系统时不会改变的
        /// CF0DA0CC-3540-11B2-A85C-BBA2377E916B
        /// </summary>
        public string ComputerSysProductUUID
        {
            get
            {
                List<string> strGrpKey = GetMocItemList(WmiClass.Win32_ComputerSystemProduct, "UUID", "");
                return strGrpKey.Count > 0 ? strGrpKey[0] : string.Empty;
            }
        }

        /// <summary>
        /// 系统类型（计算机属性-系统-系统类型）
        /// x64-based PC
        /// </summary>
        public string SystemType
        {
            get
            {
                List<string> strGrpKey = GetMocItemList(WmiClass.Win32_ComputerSystem, "SystemType", "");
                return strGrpKey.Count > 0 ? strGrpKey[0] : string.Empty;
            }
        }

        /// <summary>
        /// 计算机名称
        /// WYFPC
        /// </summary>
        public string ComputerName
        {
            get
            {
                List<string> strGrpKey = GetMocItemList(WmiClass.Win32_ComputerSystem, "Name", "");
                return strGrpKey.Count > 0 ? strGrpKey[0] : string.Empty;
                //return Environment.GetEnvironmentVariable("ComputerName");
            }
        }

        /// <summary>
        /// 用户名称
        /// WYFPC\\Elvis
        /// </summary>
        public string UserName
        {
            get
            {
                List<string> strGrpKey = GetMocItemList(WmiClass.Win32_ComputerSystem, "UserName", "");
                return strGrpKey.Count > 0 ? strGrpKey[0] : string.Empty;
            }
        }

        /// <summary>
        /// 总物理内存值
        /// 16950771712
        /// </summary>
        public string TotalPhysicalMemory
        {
            get
            {
                List<string> strGrpKey = GetMocItemList(WmiClass.Win32_ComputerSystem, "TotalPhysicalMemory", "");
                return strGrpKey.Count > 0 ? strGrpKey[0] : string.Empty;
            }
        }
    }

    /// <summary>
    /// Wmi - 计算机网卡
    /// </summary>
    public partial class WmiService
    {
        /// <summary>
        /// 已连接网卡MAC地址
        /// </summary>
        public List<string> GetNetworkCardMacAddress
        {
            get
            {
                string strWhere = "IPEnabled = 'TRUE'";
                return GetMocItemList(WmiClass.Win32_NetworkAdapterConfiguration, "MacAddress", strWhere);
            }
        }

        /// <summary>
        /// 已连接网卡IP地址
        /// </summary>
        public List<string> GetNetworkCardIPAddress
        {
            get
            {
                List<string> InstanceItemVal = new List<string>();
                string strWhere = "IPEnabled = 'TRUE'";
                //return GetWMI(WmiClass.Win32_NetworkAdapterConfiguration, "IpAddress", strWhere);
                ManagementObjectCollection moc = GetMOC(WmiClass.Win32_NetworkAdapterConfiguration, strWhere);
                if (moc == null)
                    return InstanceItemVal;
                foreach (var mo in moc)
                {
                    InstanceItemVal.Add(((Array)mo["IpAddress"]).GetValue(0).ToMyString());
                    mo.Dispose();
                }
                moc.Dispose();
                return InstanceItemVal;
            }
        }

        /// <summary>
        /// 已连接网卡的出厂ID
        /// {C0125668-6ECA-4CF2-B034-89BF853D4193}
        /// </summary>
        public List<string> GetNetworkCardGuid
        {
            get => GetMocItemList(WmiClass.Win32_NetworkAdapterConfiguration, "SettingID", "");
        }

        /// <summary>
        /// 目标网卡查找确认
        /// </summary>
        /// <param name="MacAddress">网卡MAC地址</param>
        /// <returns></returns>
        public bool IsNetworkCardMatched(string MacAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(MacAddress))
                    return false;
                string strWhere = $"MACAddress = '{MacAddress}'";
                ManagementObjectCollection moc = GetMOC(WmiClass.Win32_NetworkAdapterConfiguration, strWhere);
                return moc.Count > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Wmi - 计算机服务 Services.msc
    /// </summary>
    public partial class WmiService
    {
        /// <summary>
        /// 获取Windows服务项
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, string>> GetWinService(string strWhere)
        {
            try
            {
                ManagementObjectCollection moc = GetMOC(WmiClass.Win32_Service, strWhere);
                if (moc == null || moc?.Count <= 0) return null;
                Dictionary<string, Dictionary<string, string>> Dic = new Dictionary<string, Dictionary<string, string>>();
                foreach (ManagementObject mo in moc)
                {
                    string strServiceName = mo["Name"].ToMyString();
                    if (strServiceName.IsEmpty()) continue;
                    if (!Dic.ContainsKey(strServiceName))
                        Dic.Add(strServiceName, new Dictionary<string, string>());
                    foreach (PropertyData pd in mo.Properties)
                    {
                        Dic[strServiceName].AppandDict(pd.Name, pd.Value.ToMyString());
                    }
                }
                return Dic;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取MySQL服务项
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,Dictionary<string,string>> GetWinServiceOfMySQL(string strWhere)
        {
            return GetWinService("PathName like '%mysqld%'");
        }
    }
}


namespace Engine.ComDriver.SPECTROL
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class AutomationEvent
    {

        private AutomationEventPara paraField;

        private string nameField;

        private string directionField;

        /// <remarks/>
        public AutomationEventPara Para
        {
            get
            {
                return this.paraField;
            }
            set
            {
                this.paraField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Direction
        {
            get
            {
                return this.directionField;
            }
            set
            {
                this.directionField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationEventPara
    {

        private AutomationEventParaRemoteControlDescriptor remoteControlDescriptorField;

        private AutomationEventParaProfileDescriptor profileDescriptorField;

        private AutomationEventParaProcessInformationDescriptor processInformationDescriptorField;

        private AutomationEventParaProcessErrorDescriptor processErrorDescriptorField;

        private AutomationEventParaOpticsDriftCorrectionDataDescriptor opticsDriftCorrectionDataDescriptorField;

        private AutomationEventParaOperatorInteractionDescriptor operatorInteractionDescriptorField;

        private AutomationEventParaMethodDescriptor methodDescriptorField;

        private AutomationEventParaICalStandardDescriptor iCalStandardDescriptorField;

        private AutomationEventParaMeasurementReplicateDescriptor measurementReplicateDescriptorField;

        private AutomationEventParaMeasurementCompletionDescriptor measurementCompletionDescriptorField;

        private AutomationEventParaMaintenanceReminderDescriptor maintenanceReminderDescriptorField;

        private AutomationEventParaLogInDescriptor logInDescriptorField;

        private AutomationEventParaInstrumentStateDescriptor instrumentStateDescriptorField;

        private AutomationEventParaBaseDataChangedDescriptor baseDataChangedDescriptorField;

        private AutomationEventParaAnalysisDescriptor analysisDescriptorField;

        /// <remarks/>
        public AutomationEventParaRemoteControlDescriptor RemoteControlDescriptor
        {
            get
            {
                return this.remoteControlDescriptorField;
            }
            set
            {
                this.remoteControlDescriptorField = value;
            }
        }

        /// <remarks/>
        public AutomationEventParaProfileDescriptor ProfileDescriptor
        {
            get
            {
                return this.profileDescriptorField;
            }
            set
            {
                this.profileDescriptorField = value;
            }
        }

        /// <remarks/>
        public AutomationEventParaProcessInformationDescriptor ProcessInformationDescriptor
        {
            get
            {
                return this.processInformationDescriptorField;
            }
            set
            {
                this.processInformationDescriptorField = value;
            }
        }

        /// <remarks/>
        public AutomationEventParaProcessErrorDescriptor ProcessErrorDescriptor
        {
            get
            {
                return this.processErrorDescriptorField;
            }
            set
            {
                this.processErrorDescriptorField = value;
            }
        }

        /// <remarks/>
        public AutomationEventParaOpticsDriftCorrectionDataDescriptor OpticsDriftCorrectionDataDescriptor
        {
            get
            {
                return this.opticsDriftCorrectionDataDescriptorField;
            }
            set
            {
                this.opticsDriftCorrectionDataDescriptorField = value;
            }
        }

        /// <remarks/>
        public AutomationEventParaOperatorInteractionDescriptor OperatorInteractionDescriptor
        {
            get
            {
                return this.operatorInteractionDescriptorField;
            }
            set
            {
                this.operatorInteractionDescriptorField = value;
            }
        }

        /// <remarks/>
        public AutomationEventParaMethodDescriptor MethodDescriptor
        {
            get
            {
                return this.methodDescriptorField;
            }
            set
            {
                this.methodDescriptorField = value;
            }
        }

        /// <remarks/>
        public AutomationEventParaICalStandardDescriptor ICalStandardDescriptor
        {
            get
            {
                return this.iCalStandardDescriptorField;
            }
            set
            {
                this.iCalStandardDescriptorField = value;
            }
        }

        /// <remarks/>
        public AutomationEventParaMeasurementReplicateDescriptor MeasurementReplicateDescriptor
        {
            get
            {
                return this.measurementReplicateDescriptorField;
            }
            set
            {
                this.measurementReplicateDescriptorField = value;
            }
        }

        /// <remarks/>
        public AutomationEventParaMeasurementCompletionDescriptor MeasurementCompletionDescriptor
        {
            get
            {
                return this.measurementCompletionDescriptorField;
            }
            set
            {
                this.measurementCompletionDescriptorField = value;
            }
        }

        /// <remarks/>
        public AutomationEventParaMaintenanceReminderDescriptor MaintenanceReminderDescriptor
        {
            get
            {
                return this.maintenanceReminderDescriptorField;
            }
            set
            {
                this.maintenanceReminderDescriptorField = value;
            }
        }

        /// <remarks/>
        public AutomationEventParaLogInDescriptor LogInDescriptor
        {
            get
            {
                return this.logInDescriptorField;
            }
            set
            {
                this.logInDescriptorField = value;
            }
        }

        /// <remarks/>
        public AutomationEventParaInstrumentStateDescriptor InstrumentStateDescriptor
        {
            get
            {
                return this.instrumentStateDescriptorField;
            }
            set
            {
                this.instrumentStateDescriptorField = value;
            }
        }

        /// <remarks/>
        public AutomationEventParaBaseDataChangedDescriptor BaseDataChangedDescriptor
        {
            get
            {
                return this.baseDataChangedDescriptorField;
            }
            set
            {
                this.baseDataChangedDescriptorField = value;
            }
        }

        /// <remarks/>
        public AutomationEventParaAnalysisDescriptor AnalysisDescriptor
        {
            get
            {
                return this.analysisDescriptorField;
            }
            set
            {
                this.analysisDescriptorField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationEventParaRemoteControlDescriptor
    {

        private string remoteControlModeField;

        private string analyticalModeField;

        /// <remarks/>
        public string RemoteControlMode
        {
            get
            {
                return this.remoteControlModeField;
            }
            set
            {
                this.remoteControlModeField = value;
            }
        }

        /// <remarks/>
        public string AnalyticalMode
        {
            get
            {
                return this.analyticalModeField;
            }
            set
            {
                this.analyticalModeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationEventParaProfileDescriptor
    {

        private string profileNameField;

        private string profileTypeField;

        private object profileDescriptionField;

        private string defaultExcitationTypeField;

        private string defaultBaseElementNameField;

        /// <remarks/>
        public string ProfileName
        {
            get
            {
                return this.profileNameField;
            }
            set
            {
                this.profileNameField = value;
            }
        }

        /// <remarks/>
        public string ProfileType
        {
            get
            {
                return this.profileTypeField;
            }
            set
            {
                this.profileTypeField = value;
            }
        }

        /// <remarks/>
        public object ProfileDescription
        {
            get
            {
                return this.profileDescriptionField;
            }
            set
            {
                this.profileDescriptionField = value;
            }
        }

        /// <remarks/>
        public string DefaultExcitationType
        {
            get
            {
                return this.defaultExcitationTypeField;
            }
            set
            {
                this.defaultExcitationTypeField = value;
            }
        }

        /// <remarks/>
        public string DefaultBaseElementName
        {
            get
            {
                return this.defaultBaseElementNameField;
            }
            set
            {
                this.defaultBaseElementNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationEventParaProcessInformationDescriptor
    {

        private string informationTypeField;

        private string informationNameField;

        /// <remarks/>
        public string InformationType
        {
            get
            {
                return this.informationTypeField;
            }
            set
            {
                this.informationTypeField = value;
            }
        }

        /// <remarks/>
        public string InformationName
        {
            get
            {
                return this.informationNameField;
            }
            set
            {
                this.informationNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationEventParaProcessErrorDescriptor
    {

        private string errorTypeField;

        private string errorNameField;

        /// <remarks/>
        public string ErrorType
        {
            get
            {
                return this.errorTypeField;
            }
            set
            {
                this.errorTypeField = value;
            }
        }

        /// <remarks/>
        public string ErrorName
        {
            get
            {
                return this.errorNameField;
            }
            set
            {
                this.errorNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationEventParaOpticsDriftCorrectionDataDescriptor
    {

        private string opticsNameField;

        private string baseElementNameField;

        private System.DateTime measurementDateTimeField;

        private decimal minimumPixelShiftField;

        private decimal maximumPixelShiftField;

        private decimal wavelengthForMinimumPixelShiftField;

        private decimal wavelengthForMaximumPixelShiftField;

        private decimal opticsMinimumWavelengthField;

        private decimal opticsMaximumWavelengthField;

        private decimal pixelShiftForOpticsMinimumWavelengthField;

        private decimal pixelShiftForOpticsMaximumWavelengthField;

        /// <remarks/>
        public string OpticsName
        {
            get
            {
                return this.opticsNameField;
            }
            set
            {
                this.opticsNameField = value;
            }
        }

        /// <remarks/>
        public string BaseElementName
        {
            get
            {
                return this.baseElementNameField;
            }
            set
            {
                this.baseElementNameField = value;
            }
        }

        /// <remarks/>
        public System.DateTime MeasurementDateTime
        {
            get
            {
                return this.measurementDateTimeField;
            }
            set
            {
                this.measurementDateTimeField = value;
            }
        }

        /// <remarks/>
        public decimal MinimumPixelShift
        {
            get
            {
                return this.minimumPixelShiftField;
            }
            set
            {
                this.minimumPixelShiftField = value;
            }
        }

        /// <remarks/>
        public decimal MaximumPixelShift
        {
            get
            {
                return this.maximumPixelShiftField;
            }
            set
            {
                this.maximumPixelShiftField = value;
            }
        }

        /// <remarks/>
        public decimal WavelengthForMinimumPixelShift
        {
            get
            {
                return this.wavelengthForMinimumPixelShiftField;
            }
            set
            {
                this.wavelengthForMinimumPixelShiftField = value;
            }
        }

        /// <remarks/>
        public decimal WavelengthForMaximumPixelShift
        {
            get
            {
                return this.wavelengthForMaximumPixelShiftField;
            }
            set
            {
                this.wavelengthForMaximumPixelShiftField = value;
            }
        }

        /// <remarks/>
        public decimal OpticsMinimumWavelength
        {
            get
            {
                return this.opticsMinimumWavelengthField;
            }
            set
            {
                this.opticsMinimumWavelengthField = value;
            }
        }

        /// <remarks/>
        public decimal OpticsMaximumWavelength
        {
            get
            {
                return this.opticsMaximumWavelengthField;
            }
            set
            {
                this.opticsMaximumWavelengthField = value;
            }
        }

        /// <remarks/>
        public decimal PixelShiftForOpticsMinimumWavelength
        {
            get
            {
                return this.pixelShiftForOpticsMinimumWavelengthField;
            }
            set
            {
                this.pixelShiftForOpticsMinimumWavelengthField = value;
            }
        }

        /// <remarks/>
        public decimal PixelShiftForOpticsMaximumWavelength
        {
            get
            {
                return this.pixelShiftForOpticsMaximumWavelengthField;
            }
            set
            {
                this.pixelShiftForOpticsMaximumWavelengthField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationEventParaOperatorInteractionDescriptor
    {

        private string interactionNameField;

        private AutomationEventParaOperatorInteractionDescriptorInteractionOptions interactionOptionsField;

        private string selectedInteractionOptionField;

        /// <remarks/>
        public string InteractionName
        {
            get
            {
                return this.interactionNameField;
            }
            set
            {
                this.interactionNameField = value;
            }
        }

        /// <remarks/>
        public AutomationEventParaOperatorInteractionDescriptorInteractionOptions InteractionOptions
        {
            get
            {
                return this.interactionOptionsField;
            }
            set
            {
                this.interactionOptionsField = value;
            }
        }

        /// <remarks/>
        public string SelectedInteractionOption
        {
            get
            {
                return this.selectedInteractionOptionField;
            }
            set
            {
                this.selectedInteractionOptionField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationEventParaOperatorInteractionDescriptorInteractionOptions
    {

        private string interactionOptionField;

        /// <remarks/>
        public string InteractionOption
        {
            get
            {
                return this.interactionOptionField;
            }
            set
            {
                this.interactionOptionField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationEventParaMethodDescriptor
    {

        private string excitationTypeField;

        private string baseElementField;

        private string methodNameField;

        /// <remarks/>
        public string ExcitationType
        {
            get
            {
                return this.excitationTypeField;
            }
            set
            {
                this.excitationTypeField = value;
            }
        }

        /// <remarks/>
        public string BaseElement
        {
            get
            {
                return this.baseElementField;
            }
            set
            {
                this.baseElementField = value;
            }
        }

        /// <remarks/>
        public string MethodName
        {
            get
            {
                return this.methodNameField;
            }
            set
            {
                this.methodNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationEventParaICalStandardDescriptor
    {

        private string iCalStandardNameField;

        private byte minMeasurementsCountField;

        /// <remarks/>
        public string ICalStandardName
        {
            get
            {
                return this.iCalStandardNameField;
            }
            set
            {
                this.iCalStandardNameField = value;
            }
        }

        /// <remarks/>
        public byte MinMeasurementsCount
        {
            get
            {
                return this.minMeasurementsCountField;
            }
            set
            {
                this.minMeasurementsCountField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationEventParaMeasurementReplicateDescriptor
    {

        private byte measurementReplicateIndexField;

        /// <remarks/>
        public byte MeasurementReplicateIndex
        {
            get
            {
                return this.measurementReplicateIndexField;
            }
            set
            {
                this.measurementReplicateIndexField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationEventParaMeasurementCompletionDescriptor
    {

        private string measurementCompletionStateField;

        private byte measurementIndexField;

        /// <remarks/>
        public string MeasurementCompletionState
        {
            get
            {
                return this.measurementCompletionStateField;
            }
            set
            {
                this.measurementCompletionStateField = value;
            }
        }

        /// <remarks/>
        public byte MeasurementIndex
        {
            get
            {
                return this.measurementIndexField;
            }
            set
            {
                this.measurementIndexField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationEventParaMaintenanceReminderDescriptor
    {

        private string maintenanceNameField;

        private System.DateTime maturityTimeField;

        private byte maturityCounterField;

        private string canSetMaintenanceDoneField;

        /// <remarks/>
        public string MaintenanceName
        {
            get
            {
                return this.maintenanceNameField;
            }
            set
            {
                this.maintenanceNameField = value;
            }
        }

        /// <remarks/>
        public System.DateTime MaturityTime
        {
            get
            {
                return this.maturityTimeField;
            }
            set
            {
                this.maturityTimeField = value;
            }
        }

        /// <remarks/>
        public byte MaturityCounter
        {
            get
            {
                return this.maturityCounterField;
            }
            set
            {
                this.maturityCounterField = value;
            }
        }

        /// <remarks/>
        public string CanSetMaintenanceDone
        {
            get
            {
                return this.canSetMaintenanceDoneField;
            }
            set
            {
                this.canSetMaintenanceDoneField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationEventParaLogInDescriptor
    {

        private string loginNameField;

        private string realNameField;

        /// <remarks/>
        public string LoginName
        {
            get
            {
                return this.loginNameField;
            }
            set
            {
                this.loginNameField = value;
            }
        }

        /// <remarks/>
        public string RealName
        {
            get
            {
                return this.realNameField;
            }
            set
            {
                this.realNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationEventParaInstrumentStateDescriptor
    {

        private string instrumentTemperatureStateField;

        private string instrumentTemperatureField;

        private string argonStateField;

        private string flushStateField;

        private string instrumentOkField;

        private string sparkStandStateField;

        private string coolingSystemStateField;

        private string generatorErrorField;

        private string generatorProcessErrorField;

        private string generatorStateField;

        private string opticsScannerStateField;

        private string sAFTDeviceStateField;

        private string sAFTDeviceErrorField;

        private string readoutSystemErrorField;

        private string readoutSystemProcessErrorField;

        private string readoutSystemStateField;

        private string uVOpticsStateField;

        private string uVOpticsTemperatureField;

        private string uVOpticsPressureField;

        private string airOpticsStateField;

        private string airOpticsTemperatureField;

        private string airOpticsPressureField;

        private string nonResettableMeasurementsCounterField;

        private string resettableMeasurementsCounterField;

        private string sparkStandPollutionCounterField;

        /// <remarks/>
        public string InstrumentTemperatureState
        {
            get
            {
                return this.instrumentTemperatureStateField;
            }
            set
            {
                this.instrumentTemperatureStateField = value;
            }
        }

        /// <remarks/>
        public string InstrumentTemperature
        {
            get
            {
                return this.instrumentTemperatureField;
            }
            set
            {
                this.instrumentTemperatureField = value;
            }
        }

        /// <remarks/>
        public string ArgonState
        {
            get
            {
                return this.argonStateField;
            }
            set
            {
                this.argonStateField = value;
            }
        }

        /// <remarks/>
        public string FlushState
        {
            get
            {
                return this.flushStateField;
            }
            set
            {
                this.flushStateField = value;
            }
        }

        /// <remarks/>
        public string InstrumentOk
        {
            get
            {
                return this.instrumentOkField;
            }
            set
            {
                this.instrumentOkField = value;
            }
        }

        /// <remarks/>
        public string SparkStandState
        {
            get
            {
                return this.sparkStandStateField;
            }
            set
            {
                this.sparkStandStateField = value;
            }
        }

        /// <remarks/>
        public string CoolingSystemState
        {
            get
            {
                return this.coolingSystemStateField;
            }
            set
            {
                this.coolingSystemStateField = value;
            }
        }

        /// <remarks/>
        public string GeneratorError
        {
            get
            {
                return this.generatorErrorField;
            }
            set
            {
                this.generatorErrorField = value;
            }
        }

        /// <remarks/>
        public string GeneratorProcessError
        {
            get
            {
                return this.generatorProcessErrorField;
            }
            set
            {
                this.generatorProcessErrorField = value;
            }
        }

        /// <remarks/>
        public string GeneratorState
        {
            get
            {
                return this.generatorStateField;
            }
            set
            {
                this.generatorStateField = value;
            }
        }

        /// <remarks/>
        public string OpticsScannerState
        {
            get
            {
                return this.opticsScannerStateField;
            }
            set
            {
                this.opticsScannerStateField = value;
            }
        }

        /// <remarks/>
        public string SAFTDeviceState
        {
            get
            {
                return this.sAFTDeviceStateField;
            }
            set
            {
                this.sAFTDeviceStateField = value;
            }
        }

        /// <remarks/>
        public string SAFTDeviceError
        {
            get
            {
                return this.sAFTDeviceErrorField;
            }
            set
            {
                this.sAFTDeviceErrorField = value;
            }
        }

        /// <remarks/>
        public string ReadoutSystemError
        {
            get
            {
                return this.readoutSystemErrorField;
            }
            set
            {
                this.readoutSystemErrorField = value;
            }
        }

        /// <remarks/>
        public string ReadoutSystemProcessError
        {
            get
            {
                return this.readoutSystemProcessErrorField;
            }
            set
            {
                this.readoutSystemProcessErrorField = value;
            }
        }

        /// <remarks/>
        public string ReadoutSystemState
        {
            get
            {
                return this.readoutSystemStateField;
            }
            set
            {
                this.readoutSystemStateField = value;
            }
        }

        /// <remarks/>
        public string UVOpticsState
        {
            get
            {
                return this.uVOpticsStateField;
            }
            set
            {
                this.uVOpticsStateField = value;
            }
        }

        /// <remarks/>
        public string UVOpticsTemperature
        {
            get
            {
                return this.uVOpticsTemperatureField;
            }
            set
            {
                this.uVOpticsTemperatureField = value;
            }
        }

        /// <remarks/>
        public string UVOpticsPressure
        {
            get
            {
                return this.uVOpticsPressureField;
            }
            set
            {
                this.uVOpticsPressureField = value;
            }
        }

        /// <remarks/>
        public string AirOpticsState
        {
            get
            {
                return this.airOpticsStateField;
            }
            set
            {
                this.airOpticsStateField = value;
            }
        }

        /// <remarks/>
        public string AirOpticsTemperature
        {
            get
            {
                return this.airOpticsTemperatureField;
            }
            set
            {
                this.airOpticsTemperatureField = value;
            }
        }

        /// <remarks/>
        public string AirOpticsPressure
        {
            get
            {
                return this.airOpticsPressureField;
            }
            set
            {
                this.airOpticsPressureField = value;
            }
        }

        /// <remarks/>
        public string NonResettableMeasurementsCounter
        {
            get
            {
                return this.nonResettableMeasurementsCounterField;
            }
            set
            {
                this.nonResettableMeasurementsCounterField = value;
            }
        }

        /// <remarks/>
        public string ResettableMeasurementsCounter
        {
            get
            {
                return this.resettableMeasurementsCounterField;
            }
            set
            {
                this.resettableMeasurementsCounterField = value;
            }
        }

        /// <remarks/>
        public string SparkStandPollutionCounter
        {
            get
            {
                return this.sparkStandPollutionCounterField;
            }
            set
            {
                this.sparkStandPollutionCounterField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationEventParaBaseDataChangedDescriptor
    {

        private string baseDataTypeField;

        /// <remarks/>
        public string BaseDataType
        {
            get
            {
                return this.baseDataTypeField;
            }
            set
            {
                this.baseDataTypeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationEventParaAnalysisDescriptor
    {

        private string analyticalModeField;

        private AutomationEventParaAnalysisDescriptorAnalysisID analysisIDField;

        /// <remarks/>
        public string AnalyticalMode
        {
            get
            {
                return this.analyticalModeField;
            }
            set
            {
                this.analyticalModeField = value;
            }
        }

        /// <remarks/>
        public AutomationEventParaAnalysisDescriptorAnalysisID AnalysisID
        {
            get
            {
                return this.analysisIDField;
            }
            set
            {
                this.analysisIDField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationEventParaAnalysisDescriptorAnalysisID
    {

        private AutomationEventParaAnalysisDescriptorAnalysisIDSampleID sampleIDField;

        /// <remarks/>
        public AutomationEventParaAnalysisDescriptorAnalysisIDSampleID SampleID
        {
            get
            {
                return this.sampleIDField;
            }
            set
            {
                this.sampleIDField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationEventParaAnalysisDescriptorAnalysisIDSampleID
    {

        private string iDNameField;

        private string iDValueField;

        private string typeField;

        private string isSampleNameField;

        private string isReadOnlyField;

        private string mustExistField;

        private string keepLastValueField;

        /// <remarks/>
        public string IDName
        {
            get
            {
                return this.iDNameField;
            }
            set
            {
                this.iDNameField = value;
            }
        }

        /// <remarks/>
        public string IDValue
        {
            get
            {
                return this.iDValueField;
            }
            set
            {
                this.iDValueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string IsSampleName
        {
            get
            {
                return this.isSampleNameField;
            }
            set
            {
                this.isSampleNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string IsReadOnly
        {
            get
            {
                return this.isReadOnlyField;
            }
            set
            {
                this.isReadOnlyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MustExist
        {
            get
            {
                return this.mustExistField;
            }
            set
            {
                this.mustExistField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string KeepLastValue
        {
            get
            {
                return this.keepLastValueField;
            }
            set
            {
                this.keepLastValueField = value;
            }
        }
    }


}

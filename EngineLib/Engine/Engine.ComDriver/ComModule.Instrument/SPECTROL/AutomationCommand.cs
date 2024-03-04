
namespace Engine.ComDriver.SPECTROL
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class AutomationCommand
    {

        private AutomationCommandPara paraField;

        private AutomationCommandReturn returnField;

        private string nameField;

        private string directionField;

        /// <remarks/>
        public AutomationCommandPara Para
        {
            get
            {
                if (this.paraField == null)
                    this.paraField = new AutomationCommandPara();
                return this.paraField;
            }
            set
            {
                this.paraField = value;
            }
        }

        /// <remarks/>
        public AutomationCommandReturn Return
        {
            get
            {
                return this.returnField;
            }
            set
            {
                this.returnField = value;
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
    public partial class AutomationCommandPara
    {
        private AutomationCommandParaTypeStandardDescriptor typeStandardDescriptorField;

        private AutomationCommandParaMethodDescriptor methodDescriptorField;

        private AutomationCommandParaStandardizationDescriptor standardizationDescriptorField;

        private AutomationCommandParaLogInDescriptor logInDescriptorField;

        private AutomationCommandParaAnalysisDescriptor analysisDescriptorField;

        private AutomationCommandParaSampleResultsDescriptor sampleResultsDescriptorField;

        private AutomationCommandParaInstrumentStateDescriptor instrumentStateDescriptorField;

        private string textField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        public AutomationCommandParaTypeStandardDescriptor TypeStandardDescriptor
        {
            get
            {
                return this.typeStandardDescriptorField;
            }
            set
            {
                this.typeStandardDescriptorField = value;
            }
        }

        /// <remarks/>
        public AutomationCommandParaMethodDescriptor MethodDescriptor
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
        public AutomationCommandParaStandardizationDescriptor StandardizationDescriptor
        {
            get
            {
                return this.standardizationDescriptorField;
            }
            set
            {
                this.standardizationDescriptorField = value;
            }
        }

        /// <remarks/>
        public AutomationCommandParaLogInDescriptor LogInDescriptor
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
        public AutomationCommandParaAnalysisDescriptor AnalysisDescriptor
        {
            get
            {
                if(this.analysisDescriptorField==null)
                    this.analysisDescriptorField = new AutomationCommandParaAnalysisDescriptor();
                return this.analysisDescriptorField;
            }
            set
            {
                this.analysisDescriptorField = value;
            }
        }

        /// <remarks/>
        public AutomationCommandParaSampleResultsDescriptor SampleResultsDescriptor
        {
            get
            {
                return this.sampleResultsDescriptorField;
            }
            set
            {
                this.sampleResultsDescriptorField = value;
            }
        }

        /// <remarks/>
        public AutomationCommandParaInstrumentStateDescriptor InstrumentStateDescriptor
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
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationCommandParaTypeStandardDescriptor
    {

        private string typeStandardNameField;

        /// <remarks/>
        public string TypeStandardName
        {
            get
            {
                return this.typeStandardNameField;
            }
            set
            {
                this.typeStandardNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationCommandParaMethodDescriptor
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
    public partial class AutomationCommandParaStandardizationDescriptor
    {

        private string autoAcceptValidField;

        private string autoRejectInvalidField;

        /// <remarks/>
        public string AutoAcceptValid
        {
            get
            {
                return this.autoAcceptValidField;
            }
            set
            {
                this.autoAcceptValidField = value;
            }
        }

        /// <remarks/>
        public string AutoRejectInvalid
        {
            get
            {
                return this.autoRejectInvalidField;
            }
            set
            {
                this.autoRejectInvalidField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationCommandParaLogInDescriptor
    {

        private string loginNameField;

        private string passwordField;

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
        public string Password
        {
            get
            {
                return this.passwordField;
            }
            set
            {
                this.passwordField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationCommandParaAnalysisDescriptor
    {

        private string analyticalModeField;

        private AutomationCommandParaAnalysisDescriptorSampleID[] analysisIDField;

        private string standardNameField;

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
        [System.Xml.Serialization.XmlArrayItemAttribute("SampleID", IsNullable = false)]
        public AutomationCommandParaAnalysisDescriptorSampleID[] AnalysisID
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

        /// <remarks/>
        public string StandardName
        {
            get
            {
                return this.standardNameField;
            }
            set
            {
                this.standardNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationCommandParaAnalysisDescriptorSampleID
    {

        private string iDNameField;

        private string iDValueField;

        private string typeField;

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
    }

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationCommandParaSampleResultsDescriptor
    {

        private string sampleResultsTypeField;

        private string sampleResultsValuesTypeField;

        /// <remarks/>
        public string SampleResultsType
        {
            get
            {
                return this.sampleResultsTypeField;
            }
            set
            {
                this.sampleResultsTypeField = value;
            }
        }

        /// <remarks/>
        public string SampleResultsValuesType
        {
            get
            {
                return this.sampleResultsValuesTypeField;
            }
            set
            {
                this.sampleResultsValuesTypeField = value;
            }
        }
    }


    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationCommandReturn
    {

        private AutomationCommandReturnSampleResults sampleResultsField;

        private AutomationCommandReturnMeasurementCompletionDescriptor measurementCompletionDescriptorField;

        /// <remarks/>
        public AutomationCommandReturnSampleResults SampleResults
        {
            get
            {
                return this.sampleResultsField;
            }
            set
            {
                this.sampleResultsField = value;
            }
        }
        
        /// <remarks/>
        public AutomationCommandReturnMeasurementCompletionDescriptor MeasurementCompletionDescriptor
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
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationCommandReturnSampleResults
    {

        private AutomationCommandReturnSampleResultsSampleResult sampleResultField;

        private decimal xMLVersionField;

        private System.DateTime xMLCreationDateTimeField;

        /// <remarks/>
        public AutomationCommandReturnSampleResultsSampleResult SampleResult
        {
            get
            {
                return this.sampleResultField;
            }
            set
            {
                this.sampleResultField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal XMLVersion
        {
            get
            {
                return this.xMLVersionField;
            }
            set
            {
                this.xMLVersionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime XMLCreationDateTime
        {
            get
            {
                return this.xMLCreationDateTimeField;
            }
            set
            {
                this.xMLCreationDateTimeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationCommandReturnSampleResultsSampleResult
    {

        private AutomationCommandReturnSampleResultsSampleResultSampleID[] sampleIDsField;

        private AutomationCommandReturnSampleResultsSampleResultMeasurementReplicates measurementReplicatesField;

        private AutomationCommandReturnSampleResultsSampleResultMeasurementStatistics measurementStatisticsField;

        private string nameField;

        private string operatorNameField;

        private string typeField;

        private string corrTypeField;

        private string originField;

        private string backupStatusField;

        private System.DateTime recalculationDateTimeField;

        private string methodNameField;

        private string instrumentField;

        private string reproTestTypeField;

        private string reproTestResultField;

        private string areReproTestOutliersIgnoredField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("SampleID", IsNullable = false)]
        public AutomationCommandReturnSampleResultsSampleResultSampleID[] SampleIDs
        {
            get
            {
                return this.sampleIDsField;
            }
            set
            {
                this.sampleIDsField = value;
            }
        }

        /// <remarks/>
        public AutomationCommandReturnSampleResultsSampleResultMeasurementReplicates MeasurementReplicates
        {
            get
            {
                return this.measurementReplicatesField;
            }
            set
            {
                this.measurementReplicatesField = value;
            }
        }

        /// <remarks/>
        public AutomationCommandReturnSampleResultsSampleResultMeasurementStatistics MeasurementStatistics
        {
            get
            {
                return this.measurementStatisticsField;
            }
            set
            {
                this.measurementStatisticsField = value;
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
        public string OperatorName
        {
            get
            {
                return this.operatorNameField;
            }
            set
            {
                this.operatorNameField = value;
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
        public string CorrType
        {
            get
            {
                return this.corrTypeField;
            }
            set
            {
                this.corrTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Origin
        {
            get
            {
                return this.originField;
            }
            set
            {
                this.originField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string BackupStatus
        {
            get
            {
                return this.backupStatusField;
            }
            set
            {
                this.backupStatusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime RecalculationDateTime
        {
            get
            {
                return this.recalculationDateTimeField;
            }
            set
            {
                this.recalculationDateTimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Instrument
        {
            get
            {
                return this.instrumentField;
            }
            set
            {
                this.instrumentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ReproTestType
        {
            get
            {
                return this.reproTestTypeField;
            }
            set
            {
                this.reproTestTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ReproTestResult
        {
            get
            {
                return this.reproTestResultField;
            }
            set
            {
                this.reproTestResultField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string AreReproTestOutliersIgnored
        {
            get
            {
                return this.areReproTestOutliersIgnoredField;
            }
            set
            {
                this.areReproTestOutliersIgnoredField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationCommandReturnSampleResultsSampleResultSampleID
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

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationCommandReturnSampleResultsSampleResultMeasurementReplicates
    {

        private AutomationCommandReturnSampleResultsSampleResultMeasurementReplicatesMeasurementReplicate[] measurementReplicateField;

        private string countField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("MeasurementReplicate")]
        public AutomationCommandReturnSampleResultsSampleResultMeasurementReplicatesMeasurementReplicate[] MeasurementReplicate
        {
            get
            {
                return this.measurementReplicateField;
            }
            set
            {
                this.measurementReplicateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationCommandReturnSampleResultsSampleResultMeasurementReplicatesMeasurementReplicate
    {

        private AutomationCommandReturnSampleResultsSampleResultMeasurementReplicatesMeasurementReplicateMeasurement measurementField;

        private byte noField;

        private string isDeletedField;

        private string isOutlierField;

        private System.DateTime measureDateTimeField;

        /// <remarks/>
        public AutomationCommandReturnSampleResultsSampleResultMeasurementReplicatesMeasurementReplicateMeasurement Measurement
        {
            get
            {
                return this.measurementField;
            }
            set
            {
                this.measurementField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte No
        {
            get
            {
                return this.noField;
            }
            set
            {
                this.noField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string IsDeleted
        {
            get
            {
                return this.isDeletedField;
            }
            set
            {
                this.isDeletedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string IsOutlier
        {
            get
            {
                return this.isOutlierField;
            }
            set
            {
                this.isOutlierField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime MeasureDateTime
        {
            get
            {
                return this.measureDateTimeField;
            }
            set
            {
                this.measureDateTimeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationCommandReturnSampleResultsSampleResultMeasurementReplicatesMeasurementReplicateMeasurement
    {

        private object linesField;

        private AutomationCommandReturnSampleResultsSampleResultMeasurementReplicatesMeasurementReplicateMeasurementElement[] elementsField;

        private string checkTypeField;

        private string rsdCheckField;

        /// <remarks/>
        public object Lines
        {
            get
            {
                return this.linesField;
            }
            set
            {
                this.linesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Element", IsNullable = false)]
        public AutomationCommandReturnSampleResultsSampleResultMeasurementReplicatesMeasurementReplicateMeasurementElement[] Elements
        {
            get
            {
                return this.elementsField;
            }
            set
            {
                this.elementsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CheckType
        {
            get
            {
                return this.checkTypeField;
            }
            set
            {
                this.checkTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RsdCheck
        {
            get
            {
                return this.rsdCheckField;
            }
            set
            {
                this.rsdCheckField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationCommandReturnSampleResultsSampleResultMeasurementReplicatesMeasurementReplicateMeasurementElement
    {

        private string[] lineNamesField;

        private AutomationCommandReturnSampleResultsSampleResultMeasurementReplicatesMeasurementReplicateMeasurementElementElementResult[] elementResultField;

        private string elementNameField;

        private string typeField;

        private string selectedLineNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("LineName", IsNullable = false)]
        public string[] LineNames
        {
            get
            {
                return this.lineNamesField;
            }
            set
            {
                this.lineNamesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ElementResult")]
        public AutomationCommandReturnSampleResultsSampleResultMeasurementReplicatesMeasurementReplicateMeasurementElementElementResult[] ElementResult
        {
            get
            {
                return this.elementResultField;
            }
            set
            {
                this.elementResultField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ElementName
        {
            get
            {
                return this.elementNameField;
            }
            set
            {
                this.elementNameField = value;
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
        public string SelectedLineName
        {
            get
            {
                return this.selectedLineNameField;
            }
            set
            {
                this.selectedLineNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationCommandReturnSampleResultsSampleResultMeasurementReplicatesMeasurementReplicateMeasurementElementElementResult
    {

        private string resultValueField;

        private object resultValueLimitsField;

        private string typeField;

        private string kindField;

        private string unitField;

        private string displayUnitField;

        private string statTypeField;

        private string isDeletedField;

        private string statusField;

        private string extStatusField;

        private string calibrationStatusField;

        private string acceptanceStatusField;

        private string warningStatusField;

        /// <remarks/>
        public string ResultValue
        {
            get
            {
                return this.resultValueField;
            }
            set
            {
                this.resultValueField = value;
            }
        }

        /// <remarks/>
        public object ResultValueLimits
        {
            get
            {
                return this.resultValueLimitsField;
            }
            set
            {
                this.resultValueLimitsField = value;
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
        public string Kind
        {
            get
            {
                return this.kindField;
            }
            set
            {
                this.kindField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Unit
        {
            get
            {
                return this.unitField;
            }
            set
            {
                this.unitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DisplayUnit
        {
            get
            {
                return this.displayUnitField;
            }
            set
            {
                this.displayUnitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string StatType
        {
            get
            {
                return this.statTypeField;
            }
            set
            {
                this.statTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string IsDeleted
        {
            get
            {
                return this.isDeletedField;
            }
            set
            {
                this.isDeletedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ExtStatus
        {
            get
            {
                return this.extStatusField;
            }
            set
            {
                this.extStatusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CalibrationStatus
        {
            get
            {
                return this.calibrationStatusField;
            }
            set
            {
                this.calibrationStatusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string AcceptanceStatus
        {
            get
            {
                return this.acceptanceStatusField;
            }
            set
            {
                this.acceptanceStatusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string WarningStatus
        {
            get
            {
                return this.warningStatusField;
            }
            set
            {
                this.warningStatusField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationCommandReturnSampleResultsSampleResultMeasurementStatistics
    {

        private AutomationCommandReturnSampleResultsSampleResultMeasurementStatisticsMeasurement measurementField;

        /// <remarks/>
        public AutomationCommandReturnSampleResultsSampleResultMeasurementStatisticsMeasurement Measurement
        {
            get
            {
                return this.measurementField;
            }
            set
            {
                this.measurementField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationCommandReturnSampleResultsSampleResultMeasurementStatisticsMeasurement
    {

        private object linesField;

        private object elementsField;

        private string checkTypeField;

        private string rsdCheckField;

        /// <remarks/>
        public object Lines
        {
            get
            {
                return this.linesField;
            }
            set
            {
                this.linesField = value;
            }
        }

        /// <remarks/>
        public object Elements
        {
            get
            {
                return this.elementsField;
            }
            set
            {
                this.elementsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CheckType
        {
            get
            {
                return this.checkTypeField;
            }
            set
            {
                this.checkTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string RsdCheck
        {
            get
            {
                return this.rsdCheckField;
            }
            set
            {
                this.rsdCheckField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationCommandReturnMeasurementCompletionDescriptor
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
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AutomationCommandParaInstrumentStateDescriptor
    {

        private string instrumentReadyField;

        private string sparkHeaderReadyField;

        private string sparkingField;

        private string allReadyField;

        private string alarmListField;

        private string instrumentTemperatureStateField;

        private string instrumentTemperatureField;

        private string argonStateField;

        private string flushStateField;

        private string instrumentOkField;

        private string sparkStandStateField;

        private string sAFTDeviceStateField;

        /// <remarks/>
        public string InstrumentReady
        {
            get
            {
                return this.instrumentReadyField;
            }
            set
            {
                this.instrumentReadyField = value;
            }
        }

        /// <remarks/>
        public string SparkHeaderReady
        {
            get
            {
                return this.sparkHeaderReadyField;
            }
            set
            {
                this.sparkHeaderReadyField = value;
            }
        }

        /// <remarks/>
        public string Sparking
        {
            get
            {
                return this.sparkingField;
            }
            set
            {
                this.sparkingField = value;
            }
        }

        /// <remarks/>
        public string AllReady
        {
            get
            {
                return this.allReadyField;
            }
            set
            {
                this.allReadyField = value;
            }
        }

        /// <remarks/>
        public string AlarmList
        {
            get
            {
                return this.alarmListField;
            }
            set
            {
                this.alarmListField = value;
            }
        }

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
    }
}

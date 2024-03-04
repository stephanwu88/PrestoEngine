﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace Engine.WpfBase
{
    /// <summary> 显示枚举中所有选项 </summary>
    public class EnumSourceExtension : MarkupExtension
    {
        private Type _enumType;

        public Type EnumType
        {
            get { return this._enumType; }
            set
            {
                if (value != this._enumType)
                {
                    if (null != value)
                    {
                        Type enumType = Nullable.GetUnderlyingType(value) ?? value;

                        if (!enumType.IsEnum)
                            throw new ArgumentException("Type must be for an Enum.");
                    }
                    this._enumType = value;
                }
            }
        }

        public EnumSourceExtension()
        {

        }

        public EnumSourceExtension(Type enumType)
        {
            this.EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null == this._enumType)
                throw new InvalidOperationException("This EnumType must be specified.");
            Type actualEnumType = Nullable.GetUnderlyingType(this._enumType) ?? this._enumType;
            Array enumVlues = Enum.GetValues(actualEnumType);

            if (actualEnumType == this._enumType)
                return enumVlues;

            Array tempArray = Array.CreateInstance(actualEnumType, enumVlues.Length + 1);

            enumVlues.CopyTo(tempArray, 1);

            return tempArray;


        }
    }

    /// <summary> 根据DisplayAttribute特性中组名显示选项 </summary>
    public class EnumGroupSourceExtension : MarkupExtension
    {
        private Type _enumType;

        public Type EnumType
        {
            get { return this._enumType; }
            set
            {
                if (value != this._enumType)
                {
                    if (null != value)
                    {
                        Type enumType = Nullable.GetUnderlyingType(value) ?? value;

                        if (!enumType.IsEnum)
                            throw new ArgumentException("Type must be for an Enum.");
                    }
                    this._enumType = value;
                }
            }
        }

        public string GroupName { get; set; }

        public EnumGroupSourceExtension()
        {

        }

        public EnumGroupSourceExtension(Type enumType)
        {
            this.EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null == this._enumType)
                throw new InvalidOperationException("This EnumType must be specified.");
            Type actualEnumType = Nullable.GetUnderlyingType(this._enumType) ?? this._enumType;

            var names = Enum.GetNames(actualEnumType);

            List<string> matchs = new List<string>();

            foreach (var item in names)
            {
                var field = actualEnumType.GetField(item);

                if (field == null) continue;

                var display = field.GetCustomAttribute<DisplayAttribute>();

                if (display == null) continue;

                if(display.GroupName.Split(',').Contains(this.GroupName))
                {
                    matchs.Add(item);
                }
            }

            return matchs.Select(l=>Enum.Parse(actualEnumType,l));
        }
    }

}

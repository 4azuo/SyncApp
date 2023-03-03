using System;

[AttributeUsage(AttributeTargets.Property)]
class NotifyMethodAttribute : Attribute
{
    public string[] Methods { get; set; }

    public NotifyMethodAttribute(string[] iMethods)
    {
        Methods = iMethods;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

public abstract class AutoNotifiableObject : INotifyPropertyChanged
{
    /// <summary>
    /// consts
    /// </summary>
    private const int AUTO_RENOTifY_PERIOD = 100;
    private const int AUTO_RENOTifY_MAX = 30;

    /// <summary>
    /// Events
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Variables
    /// </summary>
    private int _isPaused = 0;

    /// <summary>
    /// Properties
    /// </summary>
    private static readonly DispatcherTimer AutoUpdateTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(AUTO_RENOTifY_PERIOD), DispatcherPriority.Background, (s, e) => { }, Application.Current.Dispatcher);
    private static readonly Dictionary<PropertyInfo, MethodInfo[]> ListNotifyMethods = new Dictionary<PropertyInfo, MethodInfo[]>();
    private static readonly List<Type> LoadedTypes = new List<Type>();
    private Dictionary<PropertyInfo, object> PropertyOldValues { get; } = new Dictionary<PropertyInfo, object>();
    private List<string> PausedProperties { get; } = new List<string>();
    private bool IsPaused
    {
        get
        {
            return _isPaused > 0;
        }
    }

    /// <summary>
    /// Methods
    /// </summary>
    public void Pause()
    {
        _isPaused += 1;
    }

    public void Unpause()
    {
        if (_isPaused > 0) _isPaused -= 1;
    }

    public void Pause(string propName)
    {
        PausedProperties.Add(propName);
    }

    public void Unpause(string propName)
    {
        PausedProperties.Remove(propName);
    }

    /// <summary>
    /// NotifyPropertyChanged
    /// </summary>
    private bool NotifyPropertyChanged(PropertyInfo prop)
    {
        if (!prop.CanRead || PausedProperties.Contains(prop.Name)) return false;
        object val = HashValue(prop.GetValue(this));
        object befVal = null;
        PropertyOldValues.TryGetValue(prop, out befVal);
        if (!Equals(val, befVal))
        {
            //reassign value
            PropertyOldValues[prop] = val;

            //self
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(prop.Name));

            //notify methods
            MethodInfo[] notiMethods = null;
            ListNotifyMethods.TryGetValue(prop, out notiMethods);
            if (notiMethods != null)
            {
                foreach (MethodInfo m in notiMethods)
                {
                    m.Invoke(this, new object[] { prop.Name });
                }
            }

            return true;
        }
        return false;
    }

    private object HashValue(object iValue)
    {
        if (iValue == null) return null;
        if (typeof(IEnumerable).IsAssignableFrom(iValue.GetType()))
        {
            long sum = 0;
            long index = 0;
            foreach (var i in (IEnumerable)iValue)
            {
                sum += i?.GetHashCode() ?? index;
                index++;
            }
            return sum;
        }
        else
        {
            return iValue;
        }
    }

    /// <summary>
    /// Create AutoUpdateTimer
    /// </summary>
    private void CreateAutoUpdater()
    {
        AutoUpdateTimer.Tick += AutoUpdate;
        if (!AutoUpdateTimer.IsEnabled) AutoUpdateTimer.Start();
    }

    private void AutoUpdate(object sender, EventArgs e)
    {
        if (IsPaused) return;
        int cnt = 0;
        foreach (PropertyInfo p in GetType().GetProperties())
        {
            if (NotifyPropertyChanged(p)) cnt++;
            if (cnt >= AUTO_RENOTifY_MAX) break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void CheckNotifyMethods()
    {
        var thisType = this.GetType();
        if (LoadedTypes.Contains(thisType)) return;
        LoadedTypes.Add(thisType);

        foreach (PropertyInfo p in GetType().GetProperties())
        {
            var notiMethodAtt = p.GetCustomAttribute<NotifyMethodAttribute>();
            if (notiMethodAtt != null && notiMethodAtt.Methods?.Length > 0)
            {
                var listMethods = new List<MethodInfo>();
                foreach (string mName in notiMethodAtt.Methods)
                {
                    var runMethod = thisType.GetMethod(mName);
                    if (runMethod.GetParameters().Length != 1 && runMethod.GetParameters()[0].ParameterType != typeof(string)) throw new ArgumentException();
                    listMethods.Add(runMethod);
                }
                ListNotifyMethods.Add(p, listMethods.ToArray());
            }
        }
    }

    /// <summary>
    /// constructor
    /// </summary>
    public AutoNotifiableObject()
    {
        CheckNotifyMethods();
        CreateAutoUpdater();
    }

    /// <summary>
    /// Destructor
    /// </summary>
    ~AutoNotifiableObject()
    {
        AutoUpdateTimer.Tick -= AutoUpdate;
    }

}
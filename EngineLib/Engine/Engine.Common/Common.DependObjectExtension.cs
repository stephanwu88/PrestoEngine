using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Engine.Common
{
    /// <summary>
    /// Extension methods for <see cref="DependencyObject"/>.
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 递归法寻找视觉树下节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static T GetVisualChild<T>(this DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = GetVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the first elment with the specific type and condition under the current element in visual tree.
        /// </summary>
        /// <typeparam name="T"/><peparam/>
        /// <param name="p_element"></param>
        /// <param name="p_func"></param>
        /// <returns></returns>
        public static T GetVisualChild<T>(this DependencyObject p_element, Func<T, bool> p_func = null) where T : UIElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(p_element); i++)
            {
                UIElement child = VisualTreeHelper.GetChild(p_element, i) as FrameworkElement;

                if (child == null)
                {
                    continue;
                }

                var t = child as T;

                if (t != null && (p_func == null || p_func(t)))
                {
                    return (T)child;
                }

                var grandChild = child.GetVisualChild(p_func);
                if (grandChild != null)
                    return grandChild;
            }
            return null;
        }

        /// <summary>
        /// Get all the sub item with the specific type and condition
        /// </summary>
        /// <typeparam name="T"/><peparam/>
        /// <param name="p_element"></param>
        /// <param name="p_func"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetVisualChildren<T>(this DependencyObject p_element, Func<T, bool> p_func = null) where T : UIElement
        { 
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(p_element); i++)
            {
                UIElement child = VisualTreeHelper.GetChild(p_element, i) as FrameworkElement;

                if (child == null)
                {
                    continue;
                }

                if (child is T)
                {
                    var t = (T)child;

                    if (p_func != null && !p_func(t))
                    {
                        continue;
                    }

                    foreach (var c in child.GetVisualChildren(p_func))
                    {
                        yield return c;
                    }

                    yield return t;
                }
                else
                {
                    foreach (var c in child.GetVisualChildren(p_func))
                    {
                        yield return c;
                    }
                }
            }
        }

        /// <summary>
        /// Get the parent element matched the given type on the visual tree
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        public static T GetVisualParent<T>(this DependencyObject element) where T : DependencyObject
        {
            if (element == null) return null;
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            while ((parent != null) && !(parent is T))
            {
                DependencyObject newVisualParent = VisualTreeHelper.GetParent(parent);
                if (newVisualParent != null)
                {
                    parent = newVisualParent;
                }
                else
                {
                    // try to get the logical parent ( e.g. if in Popup)
                    if (parent is FrameworkElement)
                    {
                        parent = (parent as FrameworkElement).Parent;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return parent as T;
        }

        // GetVisualParent<T>实现相同功能
        /// <summary>
        /// 递归法获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static T GetParentOfType<T>(this DependencyObject current) where T : DependencyObject
        {
            if (current == null)
            {
                return null;
            }

            var parent = VisualTreeHelper.GetParent(current);
            if (parent is T typedParent)
            {
                return typedParent;
            }
            return GetParentOfType<T>(parent);
        }

        /// <summary>
        /// 查找Xaml 父元素
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static T FindParent<T>(this DependencyObject element) where T : new()
        {
            try
            {
                DependencyObject parent = element;
                while (element != null && parent.GetType() != typeof(T))
                {
                    parent = element;
                    element = VisualTreeHelper.GetParent(element);
                }
                return (T)(object)parent;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Get the parent element matched the given type on the visual tree by the specific condition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="p_func"></param>
        /// <returns></returns>
        public static T GetVisualParent<T>(this DependencyObject element, Func<T, bool> p_func) where T : DependencyObject
        {
            if (element == null) return null;
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            while (((parent != null) && !(parent is T)) || !p_func(parent as T))
            {
                DependencyObject newVisualParent = VisualTreeHelper.GetParent(parent);
                if (newVisualParent != null)
                {
                    parent = newVisualParent;
                }
                else
                {
                    // try to get the logical parent ( e.g. if in Popup)
                    if (parent is FrameworkElement)
                    {
                        parent = (parent as FrameworkElement).Parent;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return parent as T;
        }

        /// <summary>
        /// Returns the template element of the given name within the Control.
        /// </summary>
        public static T GetTemplateByName<T>(this Control control, string name) where T : FrameworkElement
        {
            ControlTemplate template = control.Template;
            if (template != null)
            {
                return template.FindName(name, control) as T;
            }

            return null;
        }

        /// <summary>
        /// Searches the subtree of an element (including that element) 
        /// for an element of a particluar type.
        /// </summary>
        public static T GetElement<T>(this DependencyObject element) where T : FrameworkElement
        {
            T correctlyTyped = element as T;

            if (correctlyTyped != null)
            {
                return correctlyTyped;
            }

            if (element != null)
            {
                int numChildren = VisualTreeHelper.GetChildrenCount(element);
                for (int i = 0; i < numChildren; i++)
                {
                    T child = GetElement<T>(VisualTreeHelper.GetChild(element, i) as FrameworkElement);
                    if (child != null)
                    {
                        return child;
                    }
                }

                // Popups continue in another window, jump to that tree
                Popup popup = element as Popup;

                if (popup != null)
                {
                    return GetElement<T>(popup.Child as FrameworkElement);
                }
            }

            return null;
        }


        public static IEnumerable<T> GetElements<T>(this DependencyObject element) where T : FrameworkElement
        {
            T correctlyTyped = element as T;

            if (correctlyTyped != null)
            {
               yield return correctlyTyped;
            }

            if (element != null)
            {
                int numChildren = VisualTreeHelper.GetChildrenCount(element);

                for (int i = 0; i < numChildren; i++)
                {
                    foreach (var item in GetElements<T>(VisualTreeHelper.GetChild(element, i) as FrameworkElement))
                    {
                        yield return item;
                    }
                }

                // Popups continue in another window, jump to that tree
                Popup popup = element as Popup;

                if (popup != null)
                {
                    foreach (var item in GetElements<T>(popup.Child as FrameworkElement))
                    {
                        yield return item;
                    }
                }
            }
        }

        public static VisualStateGroup TryGetVisualStateGroup(this DependencyObject d, string groupName)
        {
            var root = GetImplementationRoot(d);
            if (root == null)
            {
                return null;
            }

            return VisualStateManager
                .GetVisualStateGroups(root)?
                .OfType<VisualStateGroup>()
                .FirstOrDefault(group => string.CompareOrdinal(groupName, group.Name) == 0);
        }

        internal static FrameworkElement GetImplementationRoot(DependencyObject d)
        {
            return 1 == VisualTreeHelper.GetChildrenCount(d)
                ? VisualTreeHelper.GetChild(d, 0) as FrameworkElement
                : null;
        }
    }
}
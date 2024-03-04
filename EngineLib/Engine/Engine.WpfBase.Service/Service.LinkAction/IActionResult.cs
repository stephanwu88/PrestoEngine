using System;

namespace Engine.WpfBase
{
    public interface IActionResult
    {
        object View { get; set; }
        Uri Uri { get; set; }
        object ViewModel { get; set; }
    }
}

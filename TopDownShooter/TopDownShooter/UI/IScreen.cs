using System;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended.Gui;

namespace TopDownShooter.UI
{
    public interface IScreen
    {
        Screen Screen { get; }
        void LoadFromMarkup();
    }
}

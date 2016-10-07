using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CityImageViewer
{
   static class Program
   {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main()
      {
         Application.EnableVisualStyles();
         CityImageViewer civ = new CityImageViewer();
         if (!civ.IsDisposed) Application.Run(civ);
      }
   }
}
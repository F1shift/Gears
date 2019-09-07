using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Gears.Views
{
     class Utility
    {
        public static object FindPerant<T>(Element node)
        {
            node = node.Parent;
            do
            {
                if (node is T)
                {
                    return node;
                }
                node = node.Parent;
            } while (node != null);
            return node;
        }
    }
}

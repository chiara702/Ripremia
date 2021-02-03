using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace EcoServiceApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewTipiRifiuti : StackLayout {
        ClassApiParco Parchetto = new ClassApiParco();
        public ViewTipiRifiuti() {
            InitializeComponent();
        }
        public event EventHandler Click;

        public String Denominazione {
            set {
                LblDenominazione.Text = value;
            }
        }
        public Color Colore {
            set {
                FrameColor.BackgroundColor = value;
            }
        }



        private void RifiutoDettagli_Tapped(object sender, EventArgs e) {
            EventHandler handler = Click;
            handler?.Invoke(this, e);
        }

        
    
    }
}
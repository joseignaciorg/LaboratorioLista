using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;

namespace LaboratorioLista.VisualWebPart1
{
    public partial class VisualWebPart1UserControl : UserControl
    {
        //protected void Page_Load(object sender, EventArgs e)
        //{
        //}
        protected override void OnPreRender(EventArgs e)
        {
            //Creamos la variable query que va a llevar la query 
            SPQuery query = new SPQuery();

            //Insertamos la querty enb la variable query
            query.Query = @"<where><Eq><FieldRef Name=""Estado""></FieldRef><Value Type=""Choice"">Pendiente</Value></Eq></where>";

         
            //Decimos los campos que queremos consultar con la query
            query.ViewFields = @"
                                <FieldRef Name=""ID"" />

                                <FieldRef Name=""Title"" />

                                <FieldRef Name=""Peticion"" />

                                <FieldRef Name=""Realizado_x0020_por"" />

                                <FieldRef Name=""Fecha"" />

                                <FieldRef Name=""Importe"" />
                                ";

            var web = SPContext.Current.Web;
            
            //Guardamos la lista 
            var list = web.Lists["Presupuesto"];
            
            //Saco los items que quiro mediante la query anterior
            var items = list.GetItems(query);

            //guardo en la lista los items obtenidos por la query de la tabla
            lstExpenses.DataSource = items.GetDataTable();

            lstExpenses.DataBind();
        }

        private static bool IsCheked(ListViewDataItem item)
        {
            var chekBox = item.FindControl("chkUpdate")as CheckBox;
            return chekBox.Checked;
        }

        private void UpdateItems(bool aprobado)
        {
            string status = aprobado ? "Aprobado" : "Rechazado";

            var selectedItems = from item in lstExpenses.Items
                where IsCheked(item)
                select item;

            var web = SPContext.Current.Web;
            var list = web.Lists["Presupuesto"];

            foreach (var selectedItem in selectedItems)

            {

                var hiddenField = selectedItem.FindControl("hdCodigo") as HiddenField;

                int itemID;

                if (int.TryParse(hiddenField.Value, out itemID))
                {

                    SPListItem item = list.GetItemById(itemID);

                    item["Estado"] = status;

                    item.Update();

                }

            }
        }

        protected void btnAprobado_Click(object sender, EventArgs e)
        {
            UpdateItems(true);
        }

        protected void btnRechazado_Click(object sender, EventArgs e)
        {
            UpdateItems(false);
        }
    }
}

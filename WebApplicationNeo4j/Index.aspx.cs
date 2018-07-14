using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplicationNeo4j
{
    public partial class Index : System.Web.UI.Page
    {
        ConnectNeo4j conn = new ConnectNeo4j();

        protected void Page_Load(object sender, EventArgs e)
        {
            conn.Connection();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            List<MovieDim> Movies = conn.SearchMovies(this.TextBox1.Text);
            if (Movies.Count() == 1)
            {
                Response.Redirect("NeoInfo.aspx?sk=" + Movies[0].MovieSK);                
            }
            else
            {
                this.Label1.Text = "Select your movie from matches: ";

                //add the items to the panel
                for (int i = 0; i < Movies.Count; i++)
                {
                    Button lButton = new Button();
                    lButton.ID = Movies[i].MovieSK.ToString();
                    lButton.Text = Movies[i].Title;
                    lButton.CssClass = "silentButton";
                    lButton.Click += new EventHandler(this.LButton_Click);
                    Panel1.Controls.Add(lButton);
                    Panel1.Controls.Add(new LiteralControl("<br /><br />"));
                }


            }
        }

        protected void LButton_Click(object sender, EventArgs e)
        {
            Button Button = sender as Button;
            Response.Redirect("http://guides.neo4j.com/sandbox/recommendations");
        }

    }
}
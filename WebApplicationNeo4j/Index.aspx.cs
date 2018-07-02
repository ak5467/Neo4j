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
                int MovieSK = Movies[0].MovieSK;
                //find the rating of the movie
                double Rating = conn.Rating(MovieSK);

                //find movies with similar rating
                List<MovieDim> SimilarMovies = conn.SimilarMovies(Rating);

                //find movies rated by same users
                List<MovieDim> SimilarUserMovie = conn.SimilarUserMovies(MovieSK, Rating);

                //find movies in same genre 
                List<MovieDim> SameGenreMovies = conn.SameGenreMovies(MovieSK);

                //find movies that has similar tags based on tag relevance
                List<MovieDim> SameTagMovies = conn.SameTagMovies(MovieSK);

                //add label
                this.Label1.Text = Movies[0].Title;                
            }
            else
            {
                //add the items to a list
                //display the list, once clicked on the list - do everything in else part
            }


        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplicationNeo4j
{
    public partial class NeoInfo : System.Web.UI.Page
    {
        ConnectNeo4j conn = new ConnectNeo4j();
        MovieDim Movie = null;
        double Rating = 0;
        String[] GenreArray;
        protected void Page_Load(object sender, EventArgs e)
        {
            int MovieSK = Convert.ToInt32(Request.QueryString["sk"]);

            //get a new connection
            conn.Connection();

            //get the movie with SK
            Movie = conn.GetMovie(MovieSK);
            //find the rating of the movie
            Rating = conn.Rating(MovieSK);

            //get Genre of movie
            GenreArray = conn.GetGenre(MovieSK);

            //display the movie such as name, rating, Imdb url, genre
            DisplayMovieInfo(Movie, Rating);

            //find movies with similar rating
            List<MovieDim> SimilarMovies = conn.SimilarMovies(Rating, GenreArray[0], MovieSK);
            //display movies of similar rating
            DisplaySimilarRating(SimilarMovies);

            //find movies rated by same users
            List<MovieDim> SimilarUserMovie = conn.SimilarUserMovies(MovieSK, Rating);
            //display movies rated by same users
            DisplaySimilarUserMovie(SimilarUserMovie);

            //find movies in same genre 
            List<MovieDim> SameGenreMovies = conn.SameGenreMovies(MovieSK);
            //display movies in same Genre
            DisplaySameGenreMovies(SameGenreMovies);

            //find movies that has similar tags based on tag relevance
            List<MovieDim> SameTagMovies = conn.SameTagMovies(MovieSK);
            //display movies that has similar tags based on tag relevance
            DisplaySameTagMovies(SameTagMovies);
        }

        public void DisplayMovieInfo(MovieDim Movie, double Rating)
        {
            moviePanel1.CssClass = "alignClass";
            //adding movie name
            Label label1 = new Label();
            label1.Text = Movie.Title;
            label1.Font.Name = "Cursive";
            label1.Font.Size = FontUnit.XXLarge;
            label1.ForeColor = System.Drawing.Color.DarkSlateBlue;
            moviePanel1.Controls.Add(label1);
            moviePanel1.Controls.Add(new LiteralControl("<br /><br />"));

            //adding rating
            Label ratingLabel = new Label();
            ratingLabel.Text = "Rating: " + Rating;
            ratingLabel.CssClass = "ratingCss";
            moviePanel1.Controls.Add(ratingLabel);
            moviePanel1.Controls.Add(new LiteralControl("<br /><br />"));

            //adding Genre of the movie
            Label GenreLabel = new Label();
            GenreLabel.Text = "Genre: " + String.Join(" | ", GenreArray);
            GenreLabel.CssClass = "ratingCss";
            moviePanel1.Controls.Add(GenreLabel);
            moviePanel1.Controls.Add(new LiteralControl("<br /><br />"));

            //adding Imdb link            
            Label label2 = new Label();
            label2.Text = "IMDB Link: ";
            label2.CssClass = "right_align";
            label2.ForeColor = System.Drawing.Color.DarkSlateBlue;
            moviePanel1.Controls.Add(label2);
            Button linkButton1 = new Button();
            linkButton1.Text = "https://www.imdb.com/title/tt0" + Movie.ImdbId;
            linkButton1.CssClass = "silentButton";
            linkButton1.Click += linkButton1_Click;
            moviePanel1.Controls.Add(linkButton1);
            moviePanel1.Controls.Add(new LiteralControl("<br /><br />"));
        }

        /*open Imdb*/
        protected void linkButton1_Click(object sender, EventArgs e)
        {
            Button Button = sender as Button;
            Response.Redirect(Button.Text);
        }

        /*display movies of similar rating*/
        public void DisplaySimilarRating(List<MovieDim> Movies)
        {
            //adding heading
            moviePanel2.Controls.Add(new LiteralControl("<br /><br />"));
            Label l1 = new Label();
            l1.Text = "Here are some movies with similar rating and genre for you:";
            l1.CssClass = "headings";
            moviePanel2.Controls.Add(l1);
            moviePanel2.Controls.Add(new LiteralControl("<br /><br /><br />"));

            //adding movies
            for(int i=0; i<Movies.Count; i++)
            {
                Label label = new Label();
                label.Text = (i + 1) + ". " +Movies[i].Title + " (Imdb Link: http://www.imdb.com/title/tt0"+Movies[i].ImdbId + "/)";
                label.CssClass = "labelStyle";
                moviePanel2.Controls.Add(label);
                moviePanel2.Controls.Add(new LiteralControl("<br /><br />"));
            }
            
            
        }

        /*display movies rated by similar users*/
        public void DisplaySimilarUserMovie(List<MovieDim> Movies)
        {

            //adding heading
            moviePanel3.Controls.Add(new LiteralControl("<br /><br />"));
            Label l1 = new Label();
            l1.Text = "Those who rated this movie also rated these:";
            l1.CssClass = "headings";
            moviePanel3.Controls.Add(l1);
            moviePanel3.Controls.Add(new LiteralControl("<br /><br /><br />"));

            //adding movies
            for (int i = 0; i < Movies.Count; i++)
            {
                Label label = new Label();
                label.Text = (i + 1) + ". " + Movies[i].Title + " (Imdb Link: http://www.imdb.com/title/tt0" + Movies[i].ImdbId + "/)";
                label.CssClass = "labelStyle";
                moviePanel3.Controls.Add(label);
                moviePanel3.Controls.Add(new LiteralControl("<br /><br />"));
            }
        }


        /*display movies of similar genre*/
        public void DisplaySameGenreMovies(List<MovieDim> Movies)
        {
            //adding heading
            moviePanel4.Controls.Add(new LiteralControl("<br /><br />"));
            Label l1 = new Label();
            l1.Text = "Here are a few movies from similar Genre:";
            l1.CssClass = "headings";
            moviePanel4.Controls.Add(l1);
            moviePanel4.Controls.Add(new LiteralControl("<br /><br /><br />"));

            //adding movies
            for (int i = 0; i < Movies.Count; i++)
            {
                Label label = new Label();
                label.Text = (i + 1) + ". " + Movies[i].Title + " (Imdb Link: http://www.imdb.com/title/tt0" + Movies[i].ImdbId + "/)";
                label.CssClass = "labelStyle";
                moviePanel4.Controls.Add(label);
                moviePanel4.Controls.Add(new LiteralControl("<br /><br />"));
            }
        }

        /*display movies that has similar tags based on tag relevance*/
        public void DisplaySameTagMovies(List<MovieDim> Movies)
        {
            //adding heading
            moviePanel5.Controls.Add(new LiteralControl("<br /><br />"));
            Label l1 = new Label();
            l1.Text = "Below are few movies that have similar tags by users:";
            l1.CssClass = "headings";
            moviePanel5.Controls.Add(l1);
            moviePanel5.Controls.Add(new LiteralControl("<br /><br /><br />"));

            //adding movies
            for (int i = 0; i < Movies.Count; i++)
            {
                Label label = new Label();
                label.Text = (i + 1) + ". " + Movies[i].Title + " (Imdb Link: http://www.imdb.com/title/tt0" + Movies[i].ImdbId + "/)";
                label.CssClass = "labelStyle";
                moviePanel5.Controls.Add(label);
                moviePanel5.Controls.Add(new LiteralControl("<br /><br />"));
            }
        }

        protected void backButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("Index.aspx");
        }
    }
}
 
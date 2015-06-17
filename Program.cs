using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//downloaded this package after downloading the Nuget extension 
//require this using in order for the connection objects to work
using MySql.Data.MySqlClient;

namespace MysqlMusicLib
{
    class Program
    {
        /*Patricia Organ 01110489 CT546 - Assignment 5
         * This Program is designed to achieve the goals, in communicating with a Database:
         * Connect
         * Read
         * Insert
         * Update
         * Disconnect
         * I have chosen to sign up to Amazon Web Services and get familiar with using it, 
         * have created a RDB MySQL Database in my AWS account and have created my tables
         * using MySQLWorkBench, also getting familiar with a new tool
         * My database has 3 tables: Song, Artist and Album, and I have illustrated the above 
         * requirements on all tables
         */
        static void Main(string[] args)
        {
            //[Connect]
            MySqlConnection con = new MySqlConnection(@"Server=mysqlmusiclibrary.cby8b2letu5u.eu-west-1.rds.amazonaws.com;Database=MusicDB;Uid=porgan;Pwd=popassword;");

            try
            {
                //open connection
                con.Open();
                //create a command object, set it initially to an empty string and to the open connection
                MySqlCommand cmd = new MySqlCommand(" ", con);

                //[Read]
                //call this method to display the tables and reader queries after update
                reader(con);

                //ask the user for new entrys
                string artist;
                do
                {// loop is to ensure that you cant have a blank text
                    Console.WriteLine("\nPlease type a new artist name: ");
                    artist = Console.ReadLine();
                } while (artist.Length == 0);

                //loop by calling the add method that return false if exists
                while (!addArtist(cmd, artist))
                {

                    Console.WriteLine("This Artist already exists, enter another please, " +
                                        "or hit enter to continue and use your first choice");
                    string nextartist = Console.ReadLine();
                    if (nextartist.Length == 0)
                    {
                        break;
                    }
                    artist = nextartist;

                }//end while loop

                string album;
                do
                { //this loop deals with blanks not being allowed
                    Console.WriteLine("Enter new Album name");
                    album = Console.ReadLine();
                }
                while (album.Length == 0);


                while (!addAlbum(cmd, album))
                {

                    Console.WriteLine("This Album already exists, enter another please,\n" +
                                     " or hit enter to continue, and use your first choice");
                    string nextalbum = Console.ReadLine();
                    if (nextalbum.Length == 0)
                    {
                        break;
                    }
                    album = nextalbum;

                }//end while loop

                /* This section asks for Song table details that are passed to the addSong     method */
                Console.WriteLine("Enter new Song Name");
                string song = Console.ReadLine();
                Console.WriteLine("Enter new Song Length");
                string songLnt = Console.ReadLine();
                Console.WriteLine("Enter new Song Genre");
                string songGenre = Console.ReadLine();

                //this loop called the method and return false if song exists already, else break loop
                while (!addSong(cmd, song, songLnt, songGenre, artist, album))
                {

                    Console.WriteLine("This Song already exists, enter another Song Name followed by enter " +
                                "\n and then Song length and enter and" +
                                "\nSong Genre and Enter please, or hit enter to cancel");
                    string nextsong = Console.ReadLine();
                    string nextsongLnt = Console.ReadLine();
                    string nextsongGenre = Console.ReadLine();

                    if (nextsong.Length == 0)
                    {
                        break;
                    }

                }//end while loop

                //[Read]
                //call this method to display the tables and reader queries
                reader(con);

                //[Update][song]
                //ask user to select the id of the song they want to edit
                //in this example showing that option to edit just the song name
                Console.WriteLine("Choose a song id to Update the name of:");
                int id = Int32.Parse(Console.ReadLine());

                //call the check method, which will check it exists and return true
                while (!checkSong(cmd, id))
                {
                    Console.WriteLine("This song id does not exist choose another from list above:");
                    id = Int32.Parse(Console.ReadLine());

                }
                //take in the updated string and pass it to the update method
                if (checkSong(cmd, id))
                {
                    Console.WriteLine("Enter the new name of the song");
                    updateSong(cmd, id, Console.ReadLine());
                }

                //[Update][Album]
                Console.WriteLine("Choose a Album id to Update the name of:");
                int Albumid = Int32.Parse(Console.ReadLine());

                //call the check method, which will check it exists and return true
                while (!checkAlbum(cmd, Albumid))
                {
                    Console.WriteLine("This Album id does not exist choose another from list above");
                    Albumid = Int32.Parse(Console.ReadLine());

                }
                //take in the updated string and pass it to the update method
                if (checkAlbum(cmd, Albumid))
                {
                    Console.WriteLine("Enter the new name of the Album");
                    updateAlbum(cmd, Albumid, Console.ReadLine());
                }

                //[Update][Artist]
                Console.WriteLine("Choose an Artist id to Update the name of:");
                int Artistid = Int32.Parse(Console.ReadLine());

                //call the check method, which will check it exists and return true
                while (!checkArtist(cmd, Artistid))
                {
                    Console.WriteLine("This Artist id does not exist choose another from list above");
                    Artistid = Int32.Parse(Console.ReadLine());

                }
                //take in the updated string and pass it to the update method
                if (checkArtist(cmd, Artistid))
                {
                    Console.WriteLine("Enter the new name of the Artist");
                    updateArtist(cmd, Artistid, Console.ReadLine());
                }

                //[Read]
                //call this method to display the tables and reader queries after update
                reader(con);
            }
            catch (Exception mye)
            {
                Console.WriteLine("Error with database: " + mye);
            }
            finally
            {
                //this code will always be executed so can ensure the connection will get closed
                if (con != null)
                {
                    //[Disconnect]
                    con.Close();
                }
                //so the console will stay open to review results
                Console.ReadKey();
            }

        }//end Main method



        private static void updateArtist(MySqlCommand cmd, int Artistid, string name)
        {
            try
            {
                string inputtext = @"Update Artist
                                         set ArtistName = '" + name + "' where idArtist ='" + Artistid + "'";
                cmd.CommandText = inputtext;
                //[Update] database Artist table
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {   //write to the screen for better understanding but also throw exception back to Main
                Console.WriteLine("Issue with UpdateArtist method:" + e);
                // throw new Exception();
            }
        }

        private static bool checkArtist(MySqlCommand cmd, int Artistid)
        {
            try
            {   //using bool variable so have only one return at the end after the reader is closed
                bool result;
                //set up the SQL query to use to check the Artist table
                string inputtext = @"select idArtist
                                        from Artist
                                        where idArtist = '" + Artistid + "'";

                //run the query on the command object that was passed from Main
                cmd.CommandText = inputtext;
                MySqlDataReader rdr = null;
                rdr = cmd.ExecuteReader();
                //if found in table then can return true
                if (rdr.HasRows)
                    result = true;
                else
                    result = false;

                //need to ensure the reader is closed, and leave the return to the end
                rdr.Close();

                return result;
            }
            catch (Exception e)
            {   //write to the screen for better understanding but also throw exception back to Main
                Console.WriteLine("Issue with checkArtist method:" + e);
                //throw new Exception();
                return false;
            }
        }

        private static void updateAlbum(MySqlCommand cmd, int Albumid, string name)
        {
            try
            {
                string inputtext = @"Update Album
                                         set AlbumTitle = '" + name + "' where idAlbum ='" + Albumid + "'";
                cmd.CommandText = inputtext;
                //[Update] database Album table
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {   //write to the screen for better understanding but also throw exception back to Main
                Console.WriteLine("Issue with UpdateSong method:" + e);
                // throw new Exception();
            }
        }

        private static bool checkAlbum(MySqlCommand cmd, int Albumid)
        {
            try
            {
                //using bool variable so have only one return at the end after the reader is closed
                bool result;
                //set up the SQl query to use to check the Album table
                string inputtext = @"select idAlbum
                                        from Album
                                        where idAlbum = '" + Albumid + "'";

                //run the query on the command object that was passed from Main
                cmd.CommandText = inputtext;
                MySqlDataReader rdr = null;
                rdr = cmd.ExecuteReader();
                //if found in table then can return true
                if (rdr.HasRows)
                    result = true;
                else
                    result = false;

                //need to ensure the reader is closed, and leave the return to the end
                rdr.Close();

                return result;
            }
            catch (Exception e)
            {   //write to the screen for better understanding but also throw exception back to Main
                Console.WriteLine("Issue with checkAlbum method:" + e);
                // throw new Exception();
                return false;
            }
        }
        private static void updateSong(MySqlCommand cmd, int id, string name)
        {
            try
            {
                string inputtext = @"Update song
                                         set songName = '" + name + "' where idsong ='" + id + "'";
                cmd.CommandText = inputtext;
                //[Update] database song table
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {   //write to the screen for better understanding but also throw exception back to Main
                Console.WriteLine("Issue with UpdateSong method:" + e);
                throw new Exception();
            }
        }

        private static bool checkSong(MySqlCommand cmd, int id)
        {
            try
            {
                //using bool variable so have only one return at the end after the reader is closed
                bool result;
                //set up the SQl query to use to check the song table
                string inputtext = @"select idsong
                                        from song
                                        where idsong = '" + id + "'";

                //run the query on the command object that was passed from Main
                cmd.CommandText = inputtext;
                MySqlDataReader rdr = null;
                rdr = cmd.ExecuteReader();
                //if found in table then can return true
                if (rdr.HasRows)
                    result = true;
                else
                    result = false;

                //need to ensure the reader is closed, and leave the return to the end
                rdr.Close();

                return result;
            }
            catch (Exception e)
            {   //write to the screen for better understanding but also throw exception back to Main
                Console.WriteLine("Issue with checkSong method:" + e);
                throw new Exception();
            }
        }

        private static bool addArtist(MySqlCommand command, string input)
        {

            try
            {
                string inputtext = @"Select ArtistName from Artist
                                    where ArtistName ='" + input + "'";
                command.CommandText = inputtext;
                //as duplicates are not allowed can do a check to see when a new row is 
                //wanted to be added that it doesn’t already exist
                if (input == (string)command.ExecuteScalar())
                {
                    return false;
                }
                else
                {
                    //[Insert]
                    //if it does not already exist in table it can be added
                    string addstring1 = @"insert into Artist
                                    (ArtistName)
                                    values('" + input + "')";
                    command.CommandText = addstring1;
                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (MySqlException mye)
            {
                //write to the screen for better understanding but also throw exception back to Main
                Console.WriteLine("Error with Album SQL statments: " + mye);
                throw new Exception();
            }
        }//end addArtist

        private static bool addAlbum(MySqlCommand command, string input)
        {
            try
            {
                string inputtext = @"Select AlbumTitle from Album
                                    where AlbumTitle ='" + input + "'";
                command.CommandText = inputtext;
                //as duplicates are not allowed can do a check to see when a new row is 
                //wanted to be added that it doesn’t already exist
                if (input == (string)command.ExecuteScalar())
                {
                    return false;
                }
                else
                {
                    //[Insert]
                    //if it does not already exist in table it can be added
                    string addstring = @"insert into Album
                                    (AlbumTitle)
                                    values('" + input + "')";
                    command.CommandText = addstring;
                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (MySqlException mye)
            {
                //write to the screen for better understanding but also throw exception back to Main
                Console.WriteLine("Error with Artist SQL statments: " + mye);
                throw new Exception();
            }
        }//end addAlbum

        private static bool addSong(MySqlCommand command, string song, string songlt, string songgen, string art, string alb)
        {

            try
            {
                string inputtext = @"Select songName from song
                                    where songName ='" + song + "'";
                command.CommandText = inputtext;
                //as duplicates are not allowed can do a check to see when a new row is 
                //wanted to be added that it doesn’t already exist
                if (song == (string)command.ExecuteScalar())
                {
                    return false;
                }
                else
                {
                    //if it does not already exist in table it can be added
                    string addstring = @"insert into song
                                    (songName, songLength, idArtist, idAlbum, songGenre)
                                    values('" + song + "', '" + songlt + "',(Select idArtist from Artist where ArtistName ='" + art +
                                          "'), (Select idAlbum from Album where AlbumTitle = '" + alb + "'),'" + songgen + "')";
                    command.CommandText = addstring;
                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (MySqlException mye)
            {
                //write to the screen for better understanding but also throw exception back to Main
                Console.WriteLine("Error with song SQL statments: " + mye);
                throw new Exception();
            }
        }//end addSong method

        private static void reader(MySqlConnection conn)
        {
            //[Read] print to screen all SQL reader queries
            //these are SQL statements prepared as strings to be used in the command
            string readerstring = @"select 
                                        s.idsong, ar.ArtistName, s.songGenre, s.songLength
                                        from song AS s, Artist AS ar
                                        where s.idArtist = ar.idArtist 
                                        order by s.idsong ASC";
            string readArtist = @"select * from Artist order by idArtist ASC";
            string readAlbum = @"select * from Album order by idAlbum ASC";
            string readSong = @"select * from song order by idsong ASC";
            MySqlDataReader rdr = null;

            try
            {
                //create a command object, set it initially to first string
                //SQL string from 2 tables to display same as Required in Assignment 5
                MySqlCommand cmd = new MySqlCommand(readerstring, conn);

                // using this query need to execute it
                rdr = cmd.ExecuteReader();

                //loop through and print results, using the padLeft/Right function and the toString
                //to space like a table
                Console.Write("\nSong_ID".PadRight(10));
                Console.Write("ArtistName".PadLeft(15));
                Console.Write(" ".PadLeft(13) + "Genre".PadRight(20));
                Console.WriteLine("Length".PadLeft(10));
                //loops through each row in the query result
                while (rdr.Read())
                {
                    Console.Write(rdr[0].ToString().PadRight(11));
                    Console.Write(rdr[1].ToString().PadLeft(13));
                    Console.Write(" ".PadLeft(13) + rdr[2].ToString().PadRight(18));
                    Console.WriteLine(rdr[3].ToString().PadLeft(10));
                }
                //need to close reader before using it again
                rdr.Close();

                cmd.CommandText = readArtist;
                rdr = cmd.ExecuteReader();

                //loop through and print results
                Console.WriteLine("\nArtist Table\n");
                while (rdr.Read())
                {
                    Console.WriteLine(rdr[0] + ": " + rdr[1]);
                }
                //need to close reader before using it again
                rdr.Close();

                cmd.CommandText = readAlbum;
                rdr = cmd.ExecuteReader();

                //loop through and print results
                Console.WriteLine("\nAlbum Table\n");
                while (rdr.Read())
                {
                    Console.WriteLine(rdr[0] + ": " + rdr[1]);
                }

                //need to close reader before using it again
                rdr.Close();

                cmd.CommandText = readSong;
                rdr = cmd.ExecuteReader();

                //loop through and print results
                Console.WriteLine("\nSong Table\n");
                while (rdr.Read())
                {
                    Console.WriteLine(rdr[0] + ": " + rdr[1].ToString().PadRight(20) + rdr[2].ToString().PadLeft(4) +
                        "\t " + rdr[3] + "\t " + rdr[4] + "\t " + rdr[5]);
                }

            }
            catch (Exception e)
            {   //write to the screen for better understanding but also throw exception back to Main
                Console.WriteLine("Issue with reader method:" + e);
                throw new Exception();
            }
            finally
            {
                //this code will always get executed so we know the reader will get closed here
                if (rdr == null)
                {
                    Console.WriteLine("nothing to read");
                }
                else if (rdr != null)
                {
                    rdr.Close();
                }
            }

        }//end reader method

    }//end class
}//end namespace

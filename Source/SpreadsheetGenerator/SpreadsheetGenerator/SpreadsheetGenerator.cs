using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SpreadsheetGenerator
{
    class SpreadsheetGenerator
    {
        private XElement XMLfile;
        private List<string> columns;
        private LinkedList<List<string>> mayors;

        static void Main( string[] args )
        {
            string XMLFilePath;

            if( args.Length == 1 )
            {
                XMLFilePath = Path.GetFullPath( args[0] );
            }
            else
            {
                XMLFilePath = Path.GetFullPath( @"..\..\..\..\..\modded mayors snippet.xml" );
            }

            SpreadsheetGenerator generator = new SpreadsheetGenerator( XMLFilePath );

            generator.WriteCSV( Path.ChangeExtension( XMLFilePath, "csv" ) );
        }

        public SpreadsheetGenerator( string XMLFilePath )
        {
            if( !File.Exists( XMLFilePath ) )
            {
                throw new ArgumentException( "XML file does not exist: " + XMLFilePath );
            }

            XMLfile = XElement.Load( XMLFilePath );

            XElement properties = XMLfile.Element( "Complex" ).Element( "Properties" );
            if( properties.Element( "Simple" ).Attribute( "value" ).Value != "Mayors" )
            {
                Console.Error.WriteLine( "We aren't processing the Mayors <Item> from ZXRules.dat?" );
                return;
            }

            ReadColumns( properties );

            ReadRows( properties );
        }

        private void ReadColumns( XElement properties )
        {
            LinkedList<string> linkedColumns = new LinkedList<string>();

            XElement cols = ( from d in properties.Elements( "Dictionary" )
                              where d.Attribute( "name" ).Value == "Cols"
                              select d ).Single();
            /*
            int colsCount = cols.Element( "Items" ).Elements( "Item" ).Count();
            Console.WriteLine( "colsCount: " + colsCount );
            */
            foreach( var i in cols.Element( "Items" ).Elements( "Item" ) )
            {
                string col = i.Element( "Simple" ).Attribute( "value" ).Value;
                linkedColumns.AddLast( col );
            }

            columns = new List<string>( linkedColumns );    // Now we know the number of columns, we can use a List to have O(1) lookups
            Console.WriteLine( "columns: " + columns.Count );
        }

        private void ReadRows( XElement properties )
        {
            mayors = new LinkedList<List<string>>();
            XElement rows = ( from d in properties.Elements( "Dictionary" )
                              where d.Attribute( "name" ).Value == "Rows"
                              select d ).Single();

            foreach( var i in rows.Element( "Items" ).Elements( "Item" ) )
            {
                List<string> mayor = new List<string>( columns.Count );

                int c = 1;
                foreach( var r in i.Element( "SingleArray").Element( "Items" ).Elements() )
                {
                    if( r.Name == "Simple" )
                    {
                        string value = r.Attribute( "value" ).Value;
                        // Prettify IDBonusTechnologies, IDBonusEntities, Mods
                        mayor.Add( c > columns.Count - 3 ? PrettyPrint( value ) : value );
                    }
                    else
                    {
                        mayor.Add( "" );
                    }

                    c++;
                }
                /*
                // Sanity check the Item 'dictionary' of Simple:SingleArray key:value pairings?
                if( i.Element( "Simple" ).Attribute( "value" ).Value != mayor[0] )
                {
                    Console.WriteLine( "Mayor ID key does not match first column value: " + mayor[0] );
                }
                */
                mayors.AddLast( mayor );
            }

            Console.WriteLine( "mayors: " + mayors.Count );
        }

        private string PrettyPrint( string value )
        {
            /*
             * Note: "Mayor_203" has "AdvancedUnitCenter (2); OilPlatform (2);" which works,
             * while "Sweene" has "WallStone(20); GateStone(2)" which is unverified.
             * The trailing semicolon may be superfluous.
             */

            string[] list = value.Split( ';', StringSplitOptions.RemoveEmptyEntries );

            StringBuilder prettyValue = new StringBuilder();

            prettyValue.Append( '"' );
            for( int i = 0; i < list.Length; i++ )
            {
                string item = list[i].Trim();

                item = item.Replace( "ZombieVenom", "Spitter" );
                item = item.Replace( "ZombieHarpy", "Harpy" );
                item = item.Replace( "ZombieGiant", "Giant" );
                item = item.Replace( "SoldierRegular", "Soldier" );
                item = item.Replace( "ThanatosExtraAttack", "ThanatosRocket" );
                item = item.Replace( "AdvancedUnitCenter", "EngineeringCenter" );
                item = item.Replace( "TrapStakes", "WoodTraps" );
                item = item.Replace( "TrapBlades", "IronTraps" );
                item = item.Replace( "MillWood", "Windmill" );
                item = item.Replace( "MillIron", "AdvancedWindmill" );
                item = item.Replace( "DeffensesLife", "DefencesLife" );
                item = item.Replace( "ResourceCollectionCellValue", "ResourceGeneration" );

                prettyValue.Append( item );

                if( i < list.Length - 1 )
                {
                    prettyValue.Append( '\n' );
                }
            }
            prettyValue.Append( '"' );

            return prettyValue.ToString();
        }

        private void WriteCSV( string CSVFilePath )
        {
            if( File.Exists( CSVFilePath ) )
            {
                Console.WriteLine( "CSV file will be overwritten: " + CSVFilePath );
            }

            try
            {
                using StreamWriter CSVFile = File.CreateText( CSVFilePath );

                foreach( var c in columns )
                {
                    CSVFile.Write( c );
                    CSVFile.Write( ',' );
                }

                CSVFile.Write( '\n' );

                foreach( var m in mayors )
                {

                    foreach( var c in m )
                    {
                        CSVFile.Write( c );
                        CSVFile.Write( ',' );
                    }
                    CSVFile.Write( '\n' );
                }

                CSVFile.Flush();
                CSVFile.Close();
            }
            catch( Exception e )
            {
                Console.Error.WriteLine( "Problem writing CSV file: " + CSVFilePath );
                Console.Error.WriteLine( e.Message );
            }
        }
    }
}

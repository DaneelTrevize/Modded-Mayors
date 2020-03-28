using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SpreadsheetGenerator
{

    class Table
    {
        private List<string> columns;
        private LinkedList<List<string>> rows;

        internal Table( XElement properties, bool prettify )
        {
            ReadColumns( properties );

            ReadRows( properties, prettify );
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

        private void ReadRows( XElement properties, bool prettify )
        {
            rows = new LinkedList<List<string>>();
            XElement tempRows = ( from d in properties.Elements( "Dictionary" )
                              where d.Attribute( "name" ).Value == "Rows"
                              select d ).Single();

            foreach( var i in tempRows.Element( "Items" ).Elements( "Item" ) )
            {
                List<string> row = new List<string>( columns.Count );

                int c = 1;
                foreach( var r in i.Element( "SingleArray" ).Element( "Items" ).Elements() )
                {
                    if( r.Name == "Simple" )
                    {
                        string value = r.Attribute( "value" ).Value;
                        // Prettify IDBonusTechnologies, IDBonusEntities, Mods
                        row.Add( prettify && c > columns.Count - 3 ? SpreadsheetGenerator.PrettyPrint( value ) : value );
                    }
                    else
                    {
                        row.Add( "" );
                    }

                    c++;
                }
                /*
                // Sanity check the Item 'dictionary' of Simple:SingleArray key:value pairings?
                if( i.Element( "Simple" ).Attribute( "value" ).Value != row[0] )
                {
                    Console.WriteLine( "ID key does not match first column value: " + row[0] );
                }
                */
                rows.AddLast( row );
            }

            Console.WriteLine( "rows: " + rows.Count );
        }

        internal void WriteCSV( string CSVFilePath, char delimiter )
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
                    CSVFile.Write( delimiter );
                }

                CSVFile.Write( '\n' );

                foreach( var r in rows )
                {

                    foreach( var c in r )
                    {
                        CSVFile.Write( c );
                        CSVFile.Write( delimiter );
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

    class SpreadsheetGenerator
    {
        internal static string PrettyPrint( string value )
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
                item = item.Replace( "MachineGun", "Wasp" );
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

        private Dictionary<string,Table> tables;

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


            if( generator.tables.Count > 1 )
            {
                string pathPrefix = Path.ChangeExtension( XMLFilePath, null );
                foreach( string table in generator.tables.Keys )
                {
                    generator.WriteTable( table, pathPrefix + "_" + table + ".csv", '\t' );
                }
            }
            else
            {
                generator.WriteTable( "Mayors", Path.ChangeExtension( XMLFilePath, "csv" ) );
            }
        }

        public SpreadsheetGenerator( string XMLFilePath )
        {
            if( !File.Exists( XMLFilePath ) )
            {
                throw new ArgumentException( "XML file does not exist: " + XMLFilePath );
            }

            tables = new Dictionary<string,Table>();

            XElement XMLfile = XElement.Load( XMLFilePath );

            if( XMLfile.Name == "Item" )    // Assume Mayors snippet
            {
                ReadTable( XMLfile );
            }
            else
            {
                foreach( XElement item in XMLfile.Element( "Properties" ).Element( "Dictionary" ).Element( "Items" ).Elements( "Item" ) )
                {
                    ReadTable( item, false );
                }
            }
        }

        private void ReadTable( XElement item, bool prettify = true )
        {

            string name = (string) item.Element( "Simple" ).Attribute( "value" );
            Console.WriteLine( "Adding table: " + name );

            XElement properties = item.Element( "Complex" ).Element( "Properties" );
            Table table = new Table( properties, prettify );

            tables.Add( name, table );
        }

        private void WriteTable( string name, string CSVFilePath, char delimiter = ',' )
        {
            Table table;
            if( !tables.TryGetValue( name, out table ) )
            {
                throw new ArgumentException( "Table data not found for name: " + name );
            }

            table.WriteCSV( CSVFilePath, delimiter );
        }
    }
}

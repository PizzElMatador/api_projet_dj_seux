using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;


namespace RepositoryPOO
{
    /// <summary>
    /// Classe de liaison avec une BDD MySQL
    /// </summary>
    public class MysqlRepository : BDDObjectRepository
    {
        #region Attributs
        /// <summary>
        /// 
        /// </summary>
        MySqlConnection connection;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur prenant la chaine de connexion
        /// Appelle le constructeur parent
        /// </summary>
        public MysqlRepository(string connectionString) : base(connectionString)
        {
        }
        #endregion

        #region Connexion
        /// <summary>
        /// Essaie de créer la connexion
        /// Renvoie TRUE si connexion OK, FALSE sinon
        /// </summary>
        protected override bool CreateConnection()
        {
            bool conOK = false;
            try
            {
                this.connection = new MySqlConnection(ConnectionString);
                this.connection.Open();
                conOK = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return conOK;
        }
        #endregion

        #region INSERT / UPDATE
        /// <summary>
        /// Sauvegarde l'objet en BDD
        /// </summary>
        /// <param name="bDDObject">L'objet à sauvegarder</param>
        public override int SaveObject(IBDDConnector bDDObject)
        {
            int result = -1;

            if (CreateConnection()) //Si connexion BDD OK
            {
                MySqlCommand cmd;

                //Génération de la commande
                if (!bDDObject.HasId())
                {
                    cmd = new MySqlCommand(GenerateInsertCommand(bDDObject));
                    cmd.Parameters.AddRange(bDDObject.GetInsertUpdateParameters().ToArray());
                }
                else
                {
                    cmd = new MySqlCommand(GenerateUpdateCommand(bDDObject));
                    cmd.Parameters.AddRange(bDDObject.GetInsertUpdateParameters().ToArray());
                    cmd.Parameters.AddRange(bDDObject.GetPrimaryKeyParameters().ToArray());
                }

                cmd.Connection = connection;

                //Execution de la commande
                try
                {
                    result = cmd.ExecuteNonQuery();
                    //Si insert on renvoit le dernier ID inséré
                    if (result > 0 && !bDDObject.HasId())
                    {
                        result = (int)cmd.LastInsertedId;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// Génère la requête INSERT sous forme de texte paramétrée
        /// </summary>
        /// <param name="bDDObject">L'objet à partir duquel générer la requête</param>
        protected override string GenerateInsertCommand(IBDDConnector bDDObject)
        {
            Dictionary<string, string> columns = bDDObject.GetInsertUpdateColumns();

            string insertCommand = "INSERT INTO ";
            insertCommand += bDDObject.GetTableName();
            insertCommand += " (";
            insertCommand += string.Join(",", columns.Keys);
            insertCommand += ") VALUES (";
            insertCommand += string.Join(",", columns.Values);
            insertCommand += ");";

            return insertCommand;
        }

        protected override string GenerateUpdateCommand(IBDDConnector bDDObject)
        {
            string updateCommand = "UPDATE ";
            updateCommand += bDDObject.GetTableName();
            updateCommand += " SET ";

            Dictionary<string, string> columns = bDDObject.GetInsertUpdateColumns();
            for (int i = 0; i < columns.Count; i++)
            {
                if (i < columns.Count - 1)
                {
                    updateCommand += columns.Keys.ElementAt(i) + "=" + columns.Values.ElementAt(i) + ", ";
                }
                else
                {
                    updateCommand += columns.Keys.ElementAt(i) + "=" + columns.Values.ElementAt(i) + " ";
                }
            }

            updateCommand += "WHERE ";
            updateCommand += bDDObject.GetPrimaryKeyColumn().First().Key;
            updateCommand += " = ";
            updateCommand += bDDObject.GetPrimaryKeyColumn().First().Value;
            updateCommand += ";";

            return updateCommand;
        }
        #endregion

        #region SELECT
        /// <summary>
        /// Effectue un SELECT sur la clé primaire de l'objet et le rempli avec le résultat de la requête
        /// Renvoi NULL si résultat BDD VIDE
        /// </summary>
        /// <param name="bDDObject">L'objet à remplir</param>
        /// <returns>L'objet rempli</returns>
        public override IBDDConnector GetObjectById(IBDDConnector bDDObject)
        {
            if (CreateConnection())
            {
                //Génération de la commande SELECT 
                MySqlCommand cmd = new MySqlCommand(GenerateSelectByIdCommand(bDDObject));
                //Ajout des paramètres à la requête
                cmd.Parameters.AddRange(bDDObject.GetPrimaryKeyParameters().ToArray());
                cmd.Connection = connection;

                //Execution de la requête
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    //Si résultat requête non vide
                    if (reader.Read())
                    {
                        //Remplissage de l'objet
                        bDDObject.FillWithDataReader(reader);
                    }
                    else
                    {
                        //Résultat requête vide
                        bDDObject = null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }

            return bDDObject;
        }

        /// <summary>
        /// Génère la requête SELECT sous forme de texte paramétrée
        /// </summary>
        /// <param name="bDDObject">L'objet à partir duquel générer la requête</param>
        protected override string GenerateSelectByIdCommand(IBDDConnector bDDObject)
        {
            string selectCommand = "SELECT * FROM ";
            selectCommand += bDDObject.GetTableName();
            selectCommand += " WHERE ";

            Dictionary<string, string> primaryKeys = bDDObject.GetPrimaryKeyColumn();
            for (int i = 0; i < primaryKeys.Count; i++)
            {
                if (i < primaryKeys.Count - 1)
                {
                    selectCommand += primaryKeys.Keys.ElementAt(i) + "=" + primaryKeys.Values.ElementAt(i) + " AND ";
                }
                else
                {
                    selectCommand += primaryKeys.Keys.ElementAt(i) + "=" + primaryKeys.Values.ElementAt(i) + " ";
                }
            }

            selectCommand += ";";

            return selectCommand;
        }

        /// <summary>
        /// Effectue un SELECT WHERE pour chaque attribut de l'objet
        /// Renvoi liste vide si résultat BDD VIDE
        /// </summary>
        /// <param name="list">Liste à remplir</param>
        /// <param name="whereObject">Objet servant à construire les prédicats</param>
        public override List<IBDDConnector> GetByPredicate(IBDDConnector whereObject)
        {
            List<IBDDConnector> listReturn = new List<IBDDConnector>();

            if (CreateConnection())
            {
                //Génération de la commande SELECT 
                MySqlCommand cmd = new MySqlCommand(GenerateSelectByPredicateCommand(whereObject));
                //Ajout des paramètres à la requête
                cmd.Parameters.AddRange(whereObject.GetPredicateParameters().ToArray());
                cmd.Connection = connection;

                //Execution de la requête
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        IBDDConnector obj = whereObject.getNewInstance();
                        obj.FillWithDataReader(reader);
                        listReturn.Add(obj);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }

            return listReturn;
        }

        /// <summary>
        /// Génère la requête SELECT sous forme de texte paramétrée
        /// </summary>
        /// <param name="whereObject">L'objet à partir duquel générer la requête</param>
        protected override string GenerateSelectByPredicateCommand(IBDDConnector whereObject)
        {
            string selectCommand = "SELECT * FROM ";
            selectCommand += whereObject.GetTableName();


            Dictionary<string, string> columns = whereObject.GetPredicateColumns();

            if (columns.Count > 0)
            {
                selectCommand += " WHERE ";

                for (int i = 0; i < columns.Count; i++)
                {
                    if (i < columns.Count - 1)
                    {
                        selectCommand += columns.Keys.ElementAt(i) + "=" + columns.Values.ElementAt(i) + " AND ";
                    }
                    else
                    {
                        selectCommand += columns.Keys.ElementAt(i) + "=" + columns.Values.ElementAt(i) + " ";
                    }
                }
            }

            selectCommand += ";";

            return selectCommand;
        }
        #endregion

        #region DELETE
        public override bool DeleteObject(IBDDConnector bDDObject)
        {
            int result = -1;
            if (CreateConnection())
            {
                //Génération de la commande SELECT 
                MySqlCommand cmd = new MySqlCommand(GenerateDeleteCommand(bDDObject));

                //Ajout des paramètres à la requête
                cmd.Parameters.AddRange(bDDObject.GetPrimaryKeyParameters().ToArray());
                cmd.Connection = connection;

                //Execution de la requête
                try
                {
                    result = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }

            return result >= 0;
        }

        protected override string GenerateDeleteCommand(IBDDConnector bDDObject)
        {
            string deleteCommand = "DELETE FROM ";
            deleteCommand += bDDObject.GetTableName();
            deleteCommand += " WHERE ";
            deleteCommand += bDDObject.GetPrimaryKeyColumn().First().Key;
            deleteCommand += " = ";
            deleteCommand += bDDObject.GetPrimaryKeyColumn().First().Value;
            deleteCommand += ";";

            return deleteCommand;
        }
        #endregion
    }
}

using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace RepositoryPOO
{
    /// <summary>
    /// Représente un objet de BDD avec un identifiant entier autoincrémenté
    /// </summary>
    public abstract class BDDObject : IBDDConnector
    {
        #region Attributs
        /// <summary>
        /// L'identifiant en BDD
        /// </summary>
        private int id = -1;
        public int Id { get => id; set => id = value; }
        #endregion

        #region Constructeurs
        public BDDObject() { }
        #endregion

        #region SQL Commun
        /// <summary>
        /// Renvoie le nom de la table liée à l'objet
        /// </summary>
        /// <returns></returns>
        public abstract string GetTableName();
        #endregion

        #region INSERT / UPDATE      
        /// <summary>
        /// Renvoie vrai si id existant (update) false si id vide (create)
        /// </summary>
        public bool HasId()
        {
            return Id > 0;
        }

        /// <summary>
        /// Renvoie un dictionnaire contenant les noms de colonnes et paramètres associés pour construire les requêtes insert / update
        /// Format : <NomColonne, @NomParamètre> 
        /// </summary>
        public abstract Dictionary<string, string> GetInsertUpdateColumns();

        /// <summary>
        /// Renvoie la liste des paramètres pour insérer l'objet dans la table liée
        /// </summary>
        public abstract List<IDataParameter> GetInsertUpdateParameters();
        #endregion

        #region SELECT
        /// <summary>
        /// Renvoieun dictionnaire contenant les nom et paramètre de colonne PK de la table
        /// </summary>
        public virtual Dictionary<string,string> GetPrimaryKeyColumn()
        {
            return new Dictionary<string, string> { { "ID", "@ID" } };
        }

        /// <summary>
        /// Renvoie la liste des paramètres pour filtrer sur la clé primaire de l'objet
        /// </summary>
        public virtual List<IDataParameter> GetPrimaryKeyParameters()
        {
            List<IDataParameter> mySqlParameters = new List<IDataParameter>();
            mySqlParameters.Add(new MySqlParameter("@ID", MySqlDbType.Int32) { Value = Id });

            return mySqlParameters;
        }

        /// <summary>
        /// Remplie le champ ID de l'objet après une requête SELECT
        /// </summary>
        public virtual void FillWithDataReader(DbDataReader reader)
        {
            this.id = (int)reader["ID"];
        }

        public abstract Dictionary<string, string> GetPredicateColumns();

        public abstract List<IDataParameter> GetPredicateParameters();

        public abstract IBDDConnector getNewInstance();
        #endregion

    }
}

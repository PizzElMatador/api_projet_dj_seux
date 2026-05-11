using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace RepositoryPOO
{
    /// <summary>
    /// Interface décrivant le contrat qu'un objet doit respecter pour être lié à une BDD
    /// </summary>
    public interface IBDDConnector
    {
        /// <summary>
        /// Renvoie le nom de la table liée à l'objet
        /// </summary>
        string GetTableName();

        /// <summary>
        /// Renvoie un dictionnaire contenant les noms de colonnes et paramètres associés pour construire les requêtes insert / update
        /// Format : <NomColonne, @NomParamètre> 
        /// </summary>
        Dictionary<string,string> GetInsertUpdateColumns();

        /// <summary>
        /// Renvoie la liste des paramètres pour les requêtes INSERT ET UPDATE
        /// Les noms des paramètres doivent correspondre à ceux renvoyés par la fonction GetInsertUpdateParametersName
        /// </summary>
        List<IDataParameter> GetInsertUpdateParameters();

        /// <summary>
        /// Renvoie un dictionnaire contenant le nom de colonne PK et paramètre associé
        /// </summary>
        Dictionary<string, string> GetPrimaryKeyColumn();

        /// <summary>
        /// Renvoie le nom du paramètre clé primaire
        /// </summary>
        List<IDataParameter> GetPrimaryKeyParameters();

        /// <summary>
        /// Renvoie un dictionnaire contenant les colonnes WHERE et paramètres associés
        /// </summary>
        Dictionary<string, string> GetPredicateColumns();

        /// <summary>
        /// Renvoie les paramètres pour le WHERE
        /// </summary>
        List<IDataParameter> GetPredicateParameters();

        /// <summary>
        /// Rempli l'objet avec le résultat de la requête SELECT WHERE CLE PRIMAIRE
        /// </summary>
        /// <param name="reader">Le  flux de données contenant le résultat de la requête</param>
        void FillWithDataReader(DbDataReader reader);

        /// <summary>
        /// Renvoie vrai si id existant (update) false si id vide (create)
        /// </summary>
        bool HasId();

        IBDDConnector getNewInstance();
    }
}

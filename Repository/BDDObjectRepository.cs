using System.Collections.Generic;
using System.Data;

namespace RepositoryPOO
{
    /// <summary>
    /// Classe abstraite représentant un objet de liaison vers une BDD relationnelle
    /// Il faut hériter de cette classe pour créer le lien avec une BDD en particulier (Mysql, PostGres, Oracle, SQLSERVER, ...)
    /// </summary>
    public abstract class BDDObjectRepository
    {
        #region Attributs
        /// <summary>
        /// Chaine de connexion vers la BDD
        /// </summary>
        private string connectionString;
        public string ConnectionString { get => connectionString; }
        #endregion

        #region Constructeurs
        /// <summary>
        /// Constructeur avec la chaine de connexion vers la BDD
        /// </summary>
        public BDDObjectRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }
        #endregion

        #region Connexion
        /// <summary>
        /// Essaie de créer la connexion
        /// Renvoie TRUE si connexion OK, FALSE sinon
        /// </summary>
        protected abstract bool CreateConnection();
        #endregion

        #region INSERT / UPDATE
        /// <summary>
        /// Sauvegarde un objet en BDD
        /// Si l'objet a un ID --> UPDATE
        /// Si l'objet n'a pas d'ID --> INSERT
        /// Renvoie l'ID de l'objet si OK
        /// Renvoie -1 si KO
        /// </summary>
        /// <param name="bDDObject">L'objet à sauvegarder</param>
        /// <returns>L'ID de l'objet sauvegardé</returns>
        public abstract int SaveObject(IBDDConnector bDDObject);

        /// <summary>
        /// Génère la requête INSERT sous forme de texte paramétrée
        /// </summary>
        /// <param name="bDDObject">L'objet à partir duquel générer la requête</param>
        protected abstract string GenerateInsertCommand(IBDDConnector bDDObject);

        /// <summary>
        /// Génère la requête UPDATE sous forme de texte paramétrée
        /// </summary>
        /// <param name="bDDObject">L'objet à partir duquel générer la requête</param>
        protected abstract string GenerateUpdateCommand(IBDDConnector bDDObject);
        #endregion

        #region SELECT
        /// <summary>
        /// Effectue un SELECT sur la clé primaire (int) de l'objet et le rempli avec le résultat de la requête
        /// Renvoi NULL si résultat BDD VIDE
        /// </summary>
        /// <param name="bDDObject">L'objet à remplir</param>
        /// <returns>L'objet rempli</returns>
        public abstract IBDDConnector GetObjectById(IBDDConnector bDDObject);

        /// <summary>
        /// Génère la requête SELECT sous forme de texte paramétrée
        /// </summary>
        /// <param name="bDDObject">L'objet à partir duquel générer la requête</param>
        protected abstract string GenerateSelectByIdCommand(IBDDConnector bDDObject);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list">La liste à remplir</param>
        /// <param name="whereObject"></param>
        /// <returns></returns>
        public abstract List<IBDDConnector> GetByPredicate(IBDDConnector whereObject);

        /// <summary>
        /// Génère la requête SELECT WHERE sous forme de texte paramétrée
        /// </summary>
        /// <param name="bDDObject">L'objet à partir duquel générer la requête</param>
        protected abstract string GenerateSelectByPredicateCommand(IBDDConnector whereObject);
        #endregion

        #region DELETE
        /// <summary>
        /// Supprime l'objet passé en paramètre en BDD
        /// Renvoie true si OK, false si OK
        /// </summary>
        /// <param name="bDDObject">>L'objet à supprimer</param>
        public abstract bool DeleteObject(IBDDConnector bDDObject);

        /// <summary>
        /// Génère la requête DELETE sous forme de texte paramétré
        /// </summary>
        /// <param name="bDDObject">L'objet à partir duquel générer la requête</param>
        protected abstract string GenerateDeleteCommand(IBDDConnector bDDObject);
        #endregion
    }
}

using MySql.Data.MySqlClient;
using RepositoryPOO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Bibliotheque_classe_projet_Seux
{
    // Classe représentant un utilisateur de l'application
    public class Utilisateur : BDDObject
    {
        // Attributs privés
        private string nom;
        private string prenom;
        private string email;
        
        private DateTime date_inscription;
        private string aspNetUserId;
        


        // Propriétés publiques
        public string Nom { get => nom; set => nom = value; }
        public string Prenom { get => prenom; set => prenom = value; }
        public string Email { get => email; set => email = value; }
        
        public DateTime Date_inscription { get => date_inscription; set => date_inscription = value; }
        public string AspNetUserId { get => aspNetUserId; set => aspNetUserId = value; } // ajouter  19/03/26
        public string Role { get; set; } // ajouter 28/05/26


        // Constructeur par défaut
        public Utilisateur() { }

        // Constructeur avec paramètres
        public Utilisateur(string nom, string prenom, string email,DateTime date_inscription, int id_role, string aspNetUserId) : base()
        {
            this.nom = nom;
            this.prenom = prenom;
            this.email = email;
            this.date_inscription = date_inscription;
            this.aspNetUserId = aspNetUserId;
        }

        // Retourne le nom de la table dans la base de données
        public override string GetTableName()
        {
            return "Utilisateur";
        }

        // Retourne les colonnes et leurs paramètres pour les opérations INSERT/UPDATE
        public override Dictionary<string, string> GetInsertUpdateColumns()
        {
            return new Dictionary<string, string>
            {
                { "nom", "@nom" },
                { "prenom", "@prenom" },
                { "email", "@email" },
                { "date_inscription", "@date_inscription" },
                { "AspNetUserId" , "@AspNetUserId" } // ajouter  19/03/26

            };
        }

        // Retourne les paramètres MySQL pour les opérations INSERT/UPDATE
        public override List<IDataParameter> GetInsertUpdateParameters()
        {
            List<IDataParameter> mySqlParameters = new List<IDataParameter>();
            mySqlParameters.Add(new MySqlParameter("@nom", MySqlDbType.VarChar) { Value = this.Nom });
            mySqlParameters.Add(new MySqlParameter("@prenom", MySqlDbType.VarChar) { Value = this.Prenom });
            mySqlParameters.Add(new MySqlParameter("@email", MySqlDbType.VarChar) { Value = this.Email });
            mySqlParameters.Add(new MySqlParameter("@date_inscription", MySqlDbType.Date) { Value = this.Date_inscription });
            mySqlParameters.Add(new MySqlParameter("@AspNetUserId", MySqlDbType.VarChar, 255) { Value = this.AspNetUserId }); // ajouter  19/03/26

            return mySqlParameters;
        }

        // Retourne la colonne de clé primaire de la table
        public override Dictionary<string, string> GetPrimaryKeyColumn()
        {
            return new Dictionary<string, string> { { "id_utilisateur", "@id_utilisateur" } };
        }

        // Retourne les paramètres pour filtrer sur la clé primaire
        public override List<IDataParameter> GetPrimaryKeyParameters()
        {
            List<IDataParameter> mySqlParameters = new List<IDataParameter>();
            mySqlParameters.Add(new MySqlParameter("@id_utilisateur", MySqlDbType.Int32) { Value = Id });
            return mySqlParameters;
        }

        // Remplit l'objet avec les données venant du DataReader
        public override void FillWithDataReader(DbDataReader reader)
        {
            this.Id = System.Convert.ToInt32(reader["id_utilisateur"]);
            this.Nom = reader["nom"].ToString();
            this.Prenom = reader["prenom"].ToString();
            this.Email = reader["email"].ToString();
            this.Date_inscription = Convert.ToDateTime(reader["date_inscription"]);
            this.AspNetUserId = reader["AspNetUserId"].ToString()!;
            this.Role = reader["Role"]?.ToString();

        }

        // Retourne les colonnes à utiliser pour filtrer les résultats (clause WHERE)
        public override Dictionary<string, string> GetPredicateColumns()
        {
            Dictionary<string, string> columns = new Dictionary<string, string>();

            // Ajout conditionnel des colonnes selon les valeurs remplies
            if (!string.IsNullOrEmpty(Nom))
                columns.Add("nom", "@nom");

            if (!string.IsNullOrEmpty(Prenom))
                columns.Add("prenom", "@prenom");

            if (!string.IsNullOrEmpty(Email))
                columns.Add("email", "@email");

            

            if (!string.IsNullOrEmpty(AspNetUserId))
                columns.Add("AspNetUserId", "@AspNetUserId");

            return columns;
        }

        // Retourne les paramètres MySQL correspondants aux colonnes de filtrage
        public override List<IDataParameter> GetPredicateParameters()
        {
            List<IDataParameter> mySqlParameters = new List<IDataParameter>();

            // Ajout conditionnel des paramètres selon les valeurs remplies
            if (!string.IsNullOrEmpty(Nom))
                mySqlParameters.Add(new MySqlParameter("@nom", MySqlDbType.VarChar) { Value = this.Nom });

            if (!string.IsNullOrEmpty(Prenom))
                mySqlParameters.Add(new MySqlParameter("@prenom", MySqlDbType.VarChar) { Value = this.Prenom });

            if (!string.IsNullOrEmpty(Email))
                mySqlParameters.Add(new MySqlParameter("@email", MySqlDbType.VarChar) { Value = this.Email });

           
            if (!string.IsNullOrEmpty(AspNetUserId))
                mySqlParameters.Add(new MySqlParameter("@AspNetUserId", MySqlDbType.VarChar, 255) { Value = this.AspNetUserId });

            return mySqlParameters;
        }

        // Retourne une nouvelle instance vide d'Utilisateur
        public override IBDDConnector getNewInstance()
        {
            return new Utilisateur();
        }
    }
}
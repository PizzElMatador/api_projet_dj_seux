using MySql.Data.MySqlClient;
using RepositoryPOO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Bibliotheque_classe_projet_Seux
{
    public class Clients : BDDObject
    {
        private string nom;
        private string prenom;
        private string email;
        private string mot_de_passe;
        private DateTime date_inscription;
        private int id_utilisateur;

        public string Nom { get => nom; set => nom = value; }
        public string Prenom { get => prenom; set => prenom = value; }
        public string Email { get => email; set => email = value; }
        public string Mot_de_passe { get => mot_de_passe; set => mot_de_passe = value; }
        public DateTime Date_inscription { get => date_inscription; set => date_inscription = value; }
        public int Id_utilisateur { get => id_utilisateur; set => id_utilisateur = value; }

        public Clients() { }

        public Clients(string nom, string prenom, string email, string mot_de_passe, DateTime date_inscription, int id_utilisateur) : base()
        {
            this.Nom = nom;
            this.Prenom = prenom;
            this.Email = email;
            this.Mot_de_passe = mot_de_passe;
            this.Date_inscription = date_inscription;
            this.Id_utilisateur = id_utilisateur;
        }

        public override string GetTableName() => "Clients";

        public override Dictionary<string, string> GetInsertUpdateColumns()
        {
            return new Dictionary<string, string>
            {
                { "nom", "@nom" },
                { "prenom", "@prenom" },
                { "email", "@email" },
                { "mot_de_passe", "@mot_de_passe" },
                { "date_inscription", "@date_inscription" },
                { "id_utilisateur", "@id_utilisateur" }
            };
        }

        public override List<IDataParameter> GetInsertUpdateParameters()
        {
            return new List<IDataParameter>
            {
                new MySqlParameter("@nom", MySqlDbType.VarChar) { Value = this.Nom },
                new MySqlParameter("@prenom", MySqlDbType.VarChar) { Value = this.Prenom },
                new MySqlParameter("@email", MySqlDbType.VarChar) { Value = this.Email },
                new MySqlParameter("@mot_de_passe", MySqlDbType.VarChar) { Value = this.Mot_de_passe },
                new MySqlParameter("@date_inscription", MySqlDbType.Date) { Value = this.Date_inscription },
                new MySqlParameter("@id_utilisateur", MySqlDbType.Int32) { Value = this.Id_utilisateur }
            };
        }

        public override Dictionary<string, string> GetPrimaryKeyColumn()
        {
            return new Dictionary<string, string> { { "id", "@id" } };
        }

        public override List<IDataParameter> GetPrimaryKeyParameters()
        {
            return new List<IDataParameter>
            {
                new MySqlParameter("@id", MySqlDbType.Int32) { Value = Id }
            };
        }

        public override void FillWithDataReader(DbDataReader reader)
        {
            base.FillWithDataReader(reader);
            this.Nom = reader["nom"].ToString();
            this.Prenom = reader["prenom"].ToString();
            this.Email = reader["email"].ToString();
            this.Mot_de_passe = reader["mot_de_passe"].ToString();
            this.Date_inscription = Convert.ToDateTime(reader["date_inscription"]);
            this.Id_utilisateur = Convert.ToInt32(reader["id_utilisateur"]);
        }

        public override Dictionary<string, string> GetPredicateColumns()
        {
            var columns = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(Nom)) columns.Add("nom", "@nom");
            if (!string.IsNullOrEmpty(Prenom)) columns.Add("prenom", "@prenom");
            if (!string.IsNullOrEmpty(Email)) columns.Add("email", "@email");
            if (Id_utilisateur > 0) columns.Add("id_utilisateur", "@id_utilisateur");
            return columns;
        }

        public override List<IDataParameter> GetPredicateParameters()
        {
            var parameters = new List<IDataParameter>();
            if (!string.IsNullOrEmpty(Nom))
                parameters.Add(new MySqlParameter("@nom", MySqlDbType.VarChar) { Value = this.Nom });
            if (!string.IsNullOrEmpty(Prenom))
                parameters.Add(new MySqlParameter("@prenom", MySqlDbType.VarChar) { Value = this.Prenom });
            if (!string.IsNullOrEmpty(Email))
                parameters.Add(new MySqlParameter("@email", MySqlDbType.VarChar) { Value = this.Email });
            if (Id_utilisateur > 0)
                parameters.Add(new MySqlParameter("@id_utilisateur", MySqlDbType.Int32) { Value = this.Id_utilisateur });
            return parameters;
        }

        public override IBDDConnector getNewInstance() => new Clients();
    }
}
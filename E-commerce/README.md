# Plateforme E-commerce pour Coopératives Marocaines

Application web complète développée avec ASP.NET WebForms et C# pour permettre aux coopératives marocaines de vendre leurs produits (agriculture, artisanat, produits du terroir) au Maroc et à l'international.

## 🎯 Fonctionnalités Principales

### Côté Utilisateur (Public)

#### Authentification
- ✅ Création de compte (email, téléphone)
- ✅ Connexion / Déconnexion
- ⏳ Réinitialisation du mot de passe (à implémenter)

#### Espace Utilisateur
- ⏳ Informations personnelles
- ⏳ Gestion des adresses de livraison
- ⏳ Historique des commandes
- ⏳ Suivi des commandes

#### Catalogue Produits
- ✅ Navigation par catégories
- ✅ Produits mis en avant
- ✅ Filtres : prix, catégorie, disponibilité
- ✅ Barre de recherche

#### Fiche Produit
- ✅ Nom, description, prix
- ✅ Variantes (taille, couleur, etc.)
- ✅ Photos HD
- ✅ Stock disponible
- ⏳ Avis clients (structure créée)

#### Panier
- ✅ Ajout / Suppression / Modification quantité
- ✅ Calcul automatique TTC + frais de livraison
- ✅ Estimation du délai de livraison

#### Commande
- ✅ Processus de commande complet
- ✅ Confirmation de commande
- ✅ Gestion des adresses de livraison

### Côté Administrateur

#### Dashboard Admin
- ✅ Vue globale : ventes, commandes, produits populaires
- ✅ Statistiques en temps réel
- ✅ Graphiques de performance

#### Gestion Produits
- ⏳ Ajouter / Modifier / Supprimer produits
- ⏳ Gestion des images
- ⏳ Gestion des variantes
- ⏳ Gestion des catégories

#### Gestion des Commandes
- ⏳ Validation / Préparation / Expédition / Livraison
- ⏳ Mise à jour des statuts
- ⏳ Impression bordereau de livraison

#### Gestion des Utilisateurs
- ⏳ Consultation
- ⏳ Désactivation / Réactivation compte

### Multivendeurs (Coopératives)
- ✅ Structure de base de données pour coopératives
- ✅ Support multivendeurs dans le schéma
- ⏳ Interface de gestion pour coopératives

## 🛠️ Technologies Utilisées

- **Backend**: ASP.NET WebForms, C#
- **Base de données**: SQL Server
- **Frontend**: HTML5, CSS3, JavaScript
- **Style**: Inspiré du template Fruitkha
- **Sécurité**: Hashage SHA256, Protection XSS/SQL Injection

## 📁 Structure du Projet

```
E-commerce/
├── App_Code/
│   ├── DbContext.cs          # Accès à la base de données
│   ├── SecurityHelper.cs     # Fonctions de sécurité
│   └── CartHelper.cs         # Gestion du panier
├── Assets/
│   ├── Styles/
│   │   └── main.css          # Styles principaux (style Fruitkha)
│   ├── Images/
│   └── Js/
├── Database/
│   ├── Setup.sql             # Schéma de base de données
│   └── SeedData.sql          # Données d'exemple
├── Pages/
│   ├── Public/
│   │   ├── Default.aspx     # Page d'accueil
│   │   ├── Login.aspx        # Connexion
│   │   ├── Register.aspx     # Inscription
│   │   ├── Shop.aspx         # Boutique
│   │   ├── ProductDetails.aspx # Détails produit
│   │   ├── Cart.aspx         # Panier
│   │   ├── Checkout.aspx     # Caisse
│   │   └── OrderConfirmation.aspx # Confirmation commande
│   └── Admin/
│       ├── Dashboard.aspx    # Tableau de bord admin
│       ├── Products.aspx     # Gestion produits
│       ├── Orders.aspx       # Gestion commandes
│       └── Users.aspx        # Gestion utilisateurs
├── Site.Master               # Master page publique
├── Admin.Master             # Master page admin
└── Web.config               # Configuration

```

## 🗄️ Base de Données

### Tables Principales

- **Users**: Utilisateurs (clients, admins, coopératives)
- **Cooperatives**: Informations des coopératives/vendeurs
- **Categories**: Catégories de produits
- **Products**: Produits
- **ProductImages**: Images supplémentaires des produits
- **ProductVariants**: Variantes (taille, couleur, etc.)
- **Orders**: Commandes
- **OrderItems**: Articles de commande
- **ShoppingCart**: Panier d'achat
- **Addresses**: Adresses de livraison
- **Reviews**: Avis clients
- **Wishlist**: Liste de souhaits

## 🚀 Installation

1. **Prérequis**
   - Visual Studio 2019 ou supérieur
   - SQL Server 2016 ou supérieur
   - .NET Framework 4.7.2

2. **Configuration de la base de données**
   - Exécuter le script `Database/Setup.sql` dans SQL Server
   - Mettre à jour la chaîne de connexion dans `Web.config`

3. **Configuration de la connexion**
   ```xml
   <connectionStrings>
     <add name="EcommerceDB" 
          connectionString="Data Source=VOTRE_SERVEUR;Initial Catalog=EcommerceDB;Integrated Security=True" 
          providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

4. **Lancer l'application**
   - Ouvrir le projet dans Visual Studio
   - Appuyer sur F5 pour démarrer

## 🔐 Sécurité

- ✅ Hashage des mots de passe (SHA256)
- ✅ Protection contre les injections SQL (paramètres)
- ✅ Protection XSS (HtmlEncode)
- ✅ Validation des entrées utilisateur
- ⏳ Certificat SSL (à configurer en production)

## 🎨 Design

Le design s'inspire du template **Fruitkha** avec :
- Couleur principale : #F28123 (orange)
- Couleur texte : #051922 (dark)
- Polices : Open Sans et Poppins
- Style moderne et professionnel
- Responsive design

## 📝 Notes Importantes

- Le mot de passe par défaut de l'admin est : `admin123` (hashé en SHA256)
- Les images de produits doivent être placées dans `/Assets/Images/Products/`
- Le panier utilise les sessions pour les utilisateurs non connectés
- Les commandes sont automatiquement liées à l'utilisateur connecté

## 🔄 Prochaines Étapes

1. Implémenter la réinitialisation de mot de passe
2. Créer l'espace utilisateur complet (profil, commandes)
3. Finaliser la gestion admin (CRUD produits, commandes)
4. Ajouter le système d'avis clients
5. Créer l'interface de gestion pour les coopératives
6. Optimiser les performances (cache, compression images)
7. Ajouter les tests unitaires

## 👥 Auteur

Développé pour les coopératives marocaines - 2024

## 📄 Licence

Projet éducatif - Tous droits réservés


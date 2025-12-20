# Plateforme E-commerce pour Coopératives Marocaines

Application web complète développée avec ASP.NET WebForms et C# pour permettre aux coopératives marocaines de vendre leurs produits (agriculture, artisanat, produits du terroir) au Maroc et à l'international.

## <i class="fas fa-bullseye"></i> Fonctionnalités Principales

### Côté Utilisateur (Public)

#### Authentification
- <i class="fas fa-check-circle"></i> Création de compte (email, téléphone)
- <i class="fas fa-check-circle"></i> Connexion / Déconnexion
- <i class="fas fa-clock"></i> Réinitialisation du mot de passe (à implémenter)

#### Espace Utilisateur
- <i class="fas fa-clock"></i> Informations personnelles
- <i class="fas fa-clock"></i> Gestion des adresses de livraison
- <i class="fas fa-clock"></i> Historique des commandes
- <i class="fas fa-clock"></i> Suivi des commandes

#### Catalogue Produits
- <i class="fas fa-check-circle"></i> Navigation par catégories
- <i class="fas fa-check-circle"></i> Produits mis en avant
- <i class="fas fa-check-circle"></i> Filtres : prix, catégorie, disponibilité
- <i class="fas fa-check-circle"></i> Barre de recherche

#### Fiche Produit
- <i class="fas fa-check-circle"></i> Nom, description, prix
- <i class="fas fa-check-circle"></i> Variantes (taille, couleur, etc.)
- <i class="fas fa-check-circle"></i> Photos HD
- <i class="fas fa-check-circle"></i> Stock disponible
- <i class="fas fa-clock"></i> Avis clients (structure créée)

#### Panier
- <i class="fas fa-check-circle"></i> Ajout / Suppression / Modification quantité
- <i class="fas fa-check-circle"></i> Calcul automatique TTC + frais de livraison
- <i class="fas fa-check-circle"></i> Estimation du délai de livraison

#### Commande
- <i class="fas fa-check-circle"></i> Processus de commande complet
- <i class="fas fa-check-circle"></i> Confirmation de commande
- <i class="fas fa-check-circle"></i> Gestion des adresses de livraison

### Côté Administrateur

#### Dashboard Admin
- <i class="fas fa-check-circle"></i> Vue globale : ventes, commandes, produits populaires
- <i class="fas fa-check-circle"></i> Statistiques en temps réel
- <i class="fas fa-check-circle"></i> Graphiques de performance

#### Gestion Produits
- <i class="fas fa-clock"></i> Ajouter / Modifier / Supprimer produits
- <i class="fas fa-clock"></i> Gestion des images
- <i class="fas fa-clock"></i> Gestion des variantes
- <i class="fas fa-clock"></i> Gestion des catégories

#### Gestion des Commandes
- <i class="fas fa-clock"></i> Validation / Préparation / Expédition / Livraison
- <i class="fas fa-clock"></i> Mise à jour des statuts
- <i class="fas fa-clock"></i> Impression bordereau de livraison

#### Gestion des Utilisateurs
- <i class="fas fa-clock"></i> Consultation
- <i class="fas fa-clock"></i> Désactivation / Réactivation compte

### Multivendeurs (Coopératives)
- <i class="fas fa-check-circle"></i> Structure de base de données pour coopératives
- <i class="fas fa-check-circle"></i> Support multivendeurs dans le schéma
- <i class="fas fa-clock"></i> Interface de gestion pour coopératives

## <i class="fas fa-tools"></i> Technologies Utilisées

- **Backend**: ASP.NET WebForms, C#
- **Base de données**: SQL Server
- **Frontend**: HTML5, CSS3, JavaScript
- **Style**: Inspiré du template Fruitkha
- **Sécurité**: Hashage SHA256, Protection XSS/SQL Injection

## <i class="fas fa-folder"></i> Structure du Projet

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

## <i class="fas fa-database"></i> Base de Données

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

## <i class="fas fa-rocket"></i> Installation

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

## <i class="fas fa-lock"></i> Sécurité

- <i class="fas fa-check-circle"></i> Hashage des mots de passe (SHA256)
- <i class="fas fa-check-circle"></i> Protection contre les injections SQL (paramètres)
- <i class="fas fa-check-circle"></i> Protection XSS (HtmlEncode)
- <i class="fas fa-check-circle"></i> Validation des entrées utilisateur
- <i class="fas fa-clock"></i> Certificat SSL (à configurer en production)

## <i class="fas fa-palette"></i> Design

Le design s'inspire du template **Fruitkha** avec :
- Couleur principale : #F28123 (orange)
- Couleur texte : #051922 (dark)
- Polices : Open Sans et Poppins
- Style moderne et professionnel
- Responsive design

## <i class="fas fa-sticky-note"></i> Notes Importantes

- Le mot de passe par défaut de l'admin est : `admin123` (hashé en SHA256)
- Les images de produits doivent être placées dans `/Assets/Images/Products/`
- Le panier utilise les sessions pour les utilisateurs non connectés
- Les commandes sont automatiquement liées à l'utilisateur connecté

## <i class="fas fa-sync-alt"></i> Prochaines Étapes

1. Implémenter la réinitialisation de mot de passe
2. Créer l'espace utilisateur complet (profil, commandes)
3. Finaliser la gestion admin (CRUD produits, commandes)
4. Ajouter le système d'avis clients
5. Créer l'interface de gestion pour les coopératives
6. Optimiser les performances (cache, compression images)
7. Ajouter les tests unitaires

## <i class="fas fa-users"></i> Auteur

Développé pour les coopératives marocaines - 2024

## <i class="fas fa-file-alt"></i> Licence

Projet éducatif - Tous droits réservés


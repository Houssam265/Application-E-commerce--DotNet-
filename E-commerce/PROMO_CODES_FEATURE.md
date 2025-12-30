# Fonctionnalité Codes Promo - Guide d'utilisation

## 📋 Vue d'ensemble

Une fonctionnalité complète de gestion des codes promo a été ajoutée à votre plateforme e-commerce. Elle permet aux administrateurs de créer et gérer des codes promo, et aux clients de les utiliser lors du checkout.

## ✨ Fonctionnalités

### Pour les Administrateurs

1. **Gestion complète des codes promo** via `/Pages/Admin/PromoCodes.aspx`
   - Ajouter de nouveaux codes promo
   - Modifier les codes existants
   - Activer/Désactiver les codes (les codes désactivés apparaissent en bas de la liste avec un style grisé)
   - Supprimer les codes promo (seulement si non utilisés)
   - Suivi de l'utilisation (nombre de fois utilisé / limite)

2. **Types de réduction supportés**
   - **Pourcentage** : Ex. 20% de réduction
   - **Montant fixe** : Ex. 50 MAD de réduction

3. **Options avancées**
   - Montant minimum du panier requis
   - Réduction maximum (pour les pourcentages)
   - Limite d'utilisation (nombre de fois que le code peut être utilisé)
   - Dates de validité (début et fin)
   - Statut actif/inactif

### Pour les Clients

1. **Interface utilisateur élégante** sur la page checkout
   - Section dédiée avec design moderne
   - Champ de saisie avec mise en forme automatique (majuscules)
   - Messages de succès/erreur clairs
   - Affichage de la réduction dans le récapitulatif

2. **Validation en temps réel**
   - Vérification de la validité du code
   - Vérification des dates
   - Vérification du montant minimum
   - Vérification de la limite d'utilisation

## 🗄️ Base de données

### Migration nécessaire

**IMPORTANT** : Vous devez exécuter le script de migration avant d'utiliser cette fonctionnalité :

```sql
-- Exécutez ce script dans SQL Server Management Studio
USE [VotreBaseDeDonnees]
GO

-- Script: Database/Migration_PromoCodes.sql
```

### Tables modifiées/créées

1. **Orders** (modifiée)
   - `DiscountAmount` (DECIMAL) : Montant de la réduction appliquée
   - `CouponCode` (NVARCHAR) : Code promo utilisé

2. **CouponUsage** (nouvelle table)
   - Suivi de l'utilisation des codes promo par commande
   - Permet de suivre quel code a été utilisé par quel utilisateur

3. **Coupons** (déjà existante)
   - Utilisée pour stocker les codes promo

## 📁 Fichiers créés/modifiés

### Nouveaux fichiers

1. `App_Code/CouponHelper.cs`
   - Classe helper pour la validation et l'application des codes promo
   - Méthode `ValidateCoupon()` : Valide un code promo et calcule la réduction
   - Méthode `RecordCouponUsage()` : Enregistre l'utilisation d'un code promo

2. `Pages/Admin/PromoCodes.aspx` et `.aspx.cs`
   - Interface d'administration complète pour gérer les codes promo
   - Liste avec tri automatique (actifs en haut, inactifs en bas)
   - Formulaire d'ajout/modification avec validation

3. `Database/Migration_PromoCodes.sql`
   - Script de migration pour ajouter les colonnes nécessaires

### Fichiers modifiés

1. `Admin.Master`
   - Ajout du lien "Codes Promo" dans le menu de navigation admin

2. `Pages/Public/Checkout.aspx`
   - Ajout de la section code promo avec design moderne
   - Affichage de la réduction dans le récapitulatif

3. `Pages/Public/Checkout.aspx.cs`
   - Intégration de la validation et application des codes promo
   - Calcul du total avec réduction
   - Enregistrement du code promo dans la commande

## 🚀 Utilisation

### Pour créer un code promo

1. Connectez-vous en tant qu'administrateur
2. Allez dans **Codes Promo** dans le menu admin
3. Cliquez sur **Nouveau Code Promo**
4. Remplissez le formulaire :
   - **Code** : Ex. WELCOME20 (sera automatiquement converti en majuscules)
   - **Type de réduction** : Pourcentage ou Montant fixe
   - **Valeur** : 20 (pour 20%) ou 50 (pour 50 MAD)
   - **Montant minimum** : Montant minimum du panier (optionnel)
   - **Réduction maximum** : Limite de réduction pour les pourcentages (optionnel)
   - **Dates** : Date de début et fin de validité
   - **Limite d'utilisation** : Nombre maximum d'utilisations (0 = illimité)
5. Cliquez sur **Enregistrer**

### Pour utiliser un code promo (client)

1. Ajoutez des produits au panier
2. Allez au checkout
3. Dans la section **Code Promo**, entrez votre code
4. Cliquez sur **Appliquer**
5. La réduction sera automatiquement appliquée au total

### Gestion des codes promo (admin)

- **Modifier** : Cliquez sur le bouton "Modifier" pour éditer un code
- **Activer/Désactiver** : Cliquez sur "Activer" ou "Désactiver" pour changer le statut
  - Les codes désactivés apparaissent en bas de la liste avec un style grisé
- **Supprimer** : Seuls les codes non utilisés peuvent être supprimés

## 🎨 Design

L'interface a été conçue pour être :
- **Moderne** : Design avec dégradés et animations
- **Intuitive** : Messages clairs et feedback visuel
- **Responsive** : S'adapte aux différentes tailles d'écran
- **Cohérent** : Suit le style général de l'application

## ⚠️ Notes importantes

1. **Migration obligatoire** : N'oubliez pas d'exécuter le script SQL avant d'utiliser la fonctionnalité
2. **Validation** : Les codes promo sont validés à chaque application (dates, limites, montant minimum)
3. **Utilisation unique par commande** : Un code promo ne peut être utilisé qu'une seule fois par commande
4. **Suivi** : L'utilisation de chaque code est enregistrée dans la table `CouponUsage`
5. **Limite d'utilisation** : Si une limite est définie, le code ne pourra plus être utilisé une fois atteint

## 🔧 Améliorations possibles

- Limiter l'utilisation par utilisateur (un code par utilisateur)
- Codes promo spécifiques à certaines catégories de produits
- Codes promo à usage unique (un code = une utilisation totale)
- Statistiques d'utilisation des codes promo dans le dashboard admin
- Export des codes promo utilisés

## 📞 Support

Si vous rencontrez des problèmes :
1. Vérifiez que la migration a été exécutée correctement
2. Vérifiez que la table `Coupons` existe bien dans votre base de données
3. Vérifiez les logs d'erreur dans Visual Studio


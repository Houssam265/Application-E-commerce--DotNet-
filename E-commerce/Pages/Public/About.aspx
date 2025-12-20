<%@ Page Title="À Propos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="About.aspx.cs" Inherits="Ecommerce.Pages.Public.About" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div style="max-width: 900px; margin: 2rem auto;">
            <div class="hero" style="margin-bottom: 4rem;">
                <h1>À Propos de Coopératives Maroc</h1>
                <p>Notre mission est de promouvoir les produits authentiques des coopératives marocaines</p>
            </div>

            <div class="card" style="margin-bottom: 2rem;">
                <h2 style="margin-bottom: 1.5rem; color: var(--primary-color);">Notre Histoire</h2>
                <p>
                    Coopératives Maroc est une plateforme e-commerce dédiée aux coopératives marocaines spécialisées dans 
                    l'agriculture biologique, l'artisanat traditionnel et les produits du terroir. Nous avons créé cette 
                    plateforme pour permettre aux coopératives de vendre leurs produits directement aux consommateurs, 
                    au Maroc et à l'international.
                </p>
                <p>
                    Notre objectif est de valoriser le savoir-faire marocain et de soutenir les producteurs locaux en leur 
                    offrant une vitrine digitale moderne et professionnelle.
                </p>
            </div>

            <div class="grid grid-3" style="margin-bottom: 2rem;">
                <div class="card" style="text-align: center;">
                    <div style="font-size: 3rem; color: var(--primary-color); margin-bottom: 1rem;">
                        <i class="fas fa-seedling"></i>
                    </div>
                    <h3>Produits Bio</h3>
                    <p>Des produits 100% biologiques et naturels directement des coopératives marocaines</p>
                </div>
                <div class="card" style="text-align: center;">
                    <div style="font-size: 3rem; color: var(--primary-color); margin-bottom: 1rem;">
                        <i class="fas fa-handshake"></i>
                    </div>
                    <h3>Commerce Équitable</h3>
                    <p>Un modèle économique qui valorise le travail des coopératives et des producteurs</p>
                </div>
                <div class="card" style="text-align: center;">
                    <div style="font-size: 3rem; color: var(--primary-color); margin-bottom: 1rem;">
                        <i class="fas fa-globe"></i>
                    </div>
                    <h3>Export International</h3>
                    <p>Livraison partout dans le monde pour promouvoir les produits marocains</p>
                </div>
            </div>

            <div class="card">
                <h2 style="margin-bottom: 1.5rem; color: var(--primary-color);">Nos Valeurs</h2>
                <ul style="list-style: none; padding: 0;">
                    <li style="padding: 1rem; margin-bottom: 0.5rem; background: var(--bg-light); border-radius: 5px;">
                        <strong><i class="fas fa-check-circle" style="color: var(--primary-color);"></i> Qualité</strong> - 
                        Nous garantissons la qualité et l'authenticité de tous nos produits
                    </li>
                    <li style="padding: 1rem; margin-bottom: 0.5rem; background: var(--bg-light); border-radius: 5px;">
                        <strong><i class="fas fa-check-circle" style="color: var(--primary-color);"></i> Transparence</strong> - 
                        Traçabilité complète de la production à la livraison
                    </li>
                    <li style="padding: 1rem; margin-bottom: 0.5rem; background: var(--bg-light); border-radius: 5px;">
                        <strong><i class="fas fa-check-circle" style="color: var(--primary-color);"></i> Durabilité</strong> - 
                        Engagement en faveur de l'agriculture durable et de l'environnement
                    </li>
                    <li style="padding: 1rem; margin-bottom: 0.5rem; background: var(--bg-light); border-radius: 5px;">
                        <strong><i class="fas fa-check-circle" style="color: var(--primary-color);"></i> Solidarité</strong> - 
                        Soutien aux coopératives et aux communautés rurales
                    </li>
                </ul>
            </div>
        </div>
    </div>
</asp:Content>


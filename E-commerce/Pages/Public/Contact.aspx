<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Contact.aspx.cs" Inherits="Ecommerce.Pages.Public.Contact" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .contact-container {
            max-width: 900px;
            margin: 2rem auto;
        }

        .contact-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 2rem;
            margin-bottom: 2rem;
        }

        .contact-info {
            padding: 2rem;
            background: var(--bg-white);
            border-radius: 12px;
            box-shadow: var(--shadow-md);
            border: 1px solid var(--border-color);
        }

        .contact-item {
            display: flex;
            align-items: start;
            gap: 1rem;
            margin-bottom: 1.5rem;
            padding: 1rem;
            border-radius: 8px;
            transition: var(--transition);
        }

        .contact-item:hover {
            background: rgba(40, 167, 69, 0.05);
            transform: translateX(5px);
        }

        .contact-icon {
            width: 50px;
            height: 50px;
            border-radius: 50%;
            background: linear-gradient(135deg, var(--primary-color), var(--primary-light));
            color: #fff;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.5rem;
            flex-shrink: 0;
            transition: var(--transition);
            box-shadow: var(--shadow-sm);
        }

        .contact-item:hover .contact-icon {
            transform: scale(1.1) rotate(5deg);
            box-shadow: var(--shadow-md);
        }

        @media (max-width: 768px) {
            .contact-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Hero Section -->
    <section class="hero">
        <div class="container">
            <h1>Contactez-nous</h1>
            <p>Nous sommes là pour vous aider et répondre à vos questions</p>
        </div>
    </section>

    <div class="container">
        <div class="contact-container">

            <div class="contact-grid">
                <div class="card">
                    <h3 style="margin-bottom: 1.5rem;">Envoyez-nous un message</h3>
                    
                    <asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success">
                        <asp:Literal ID="litSuccess" runat="server"></asp:Literal>
                    </asp:Panel>

                    <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
                        <asp:Literal ID="litError" runat="server"></asp:Literal>
                    </asp:Panel>

                    <div class="form-group">
                        <label>Nom complet *</label>
                        <asp:TextBox ID="txtName" runat="server" CssClass="form-control" required></asp:TextBox>
                    </div>

                    <asp:Panel ID="pnlEmailField" runat="server">
                        <div class="form-group">
                            <label>Email *</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" required></asp:TextBox>
                        </div>
                    </asp:Panel>

                    <div class="form-group">
                        <label>Sujet *</label>
                        <asp:TextBox ID="txtSubject" runat="server" CssClass="form-control" required></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label>Message *</label>
                        <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" required></asp:TextBox>
                    </div>

                    <asp:Button ID="btnSend" runat="server" Text="Envoyer le message" CssClass="btn btn-primary"
                        OnClick="btnSend_Click" />
                </div>

                <div>
                    <div class="contact-info">
                        <h3 style="margin-bottom: 1.5rem;">Informations de contact</h3>
                        
                        <div class="contact-item">
                            <div class="contact-icon">
                                <i class="fas fa-envelope"></i>
                            </div>
                            <div>
                                <h4 style="margin: 0 0 0.25rem 0;">Email</h4>
                                <p style="margin: 0;">contact@ecommerce.ma</p>
                            </div>
                        </div>

                        <div class="contact-item">
                            <div class="contact-icon">
                                <i class="fas fa-phone"></i>
                            </div>
                            <div>
                                <h4 style="margin: 0 0 0.25rem 0;">Téléphone</h4>
                                <p style="margin: 0;">+212 6XX XXX XXX</p>
                            </div>
                        </div>

                        <div class="contact-item">
                            <div class="contact-icon">
                                <i class="fas fa-map-marker-alt"></i>
                            </div>
                            <div>
                                <h4 style="margin: 0 0 0.25rem 0;">Adresse</h4>
                                <p style="margin: 0;">Maroc</p>
                            </div>
                        </div>

                        <div class="contact-item">
                            <div class="contact-icon">
                                <i class="fas fa-clock"></i>
                            </div>
                            <div>
                                <h4 style="margin: 0 0 0.25rem 0;">Horaires</h4>
                                <p style="margin: 0;">Lundi - Vendredi: 9h - 18h<br/>Samedi: 9h - 13h</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>


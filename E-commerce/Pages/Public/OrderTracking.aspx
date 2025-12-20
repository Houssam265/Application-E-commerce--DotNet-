<%@ Page Title="Suivi de commande" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="OrderTracking.aspx.cs" Inherits="Ecommerce.Pages.Public.OrderTracking" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .tracking-container {
            max-width: 800px;
            margin: 2rem auto;
        }

        .tracking-header {
            background: var(--bg-white);
            padding: 2rem;
            border-radius: 10px;
            border: 1px solid var(--border-color);
            margin-bottom: 2rem;
            text-align: center;
        }

        .tracking-steps {
            position: relative;
            padding: 2rem 0;
        }

        .tracking-step {
            display: flex;
            align-items: center;
            margin-bottom: 2rem;
            position: relative;
        }

        .step-icon {
            width: 50px;
            height: 50px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.5rem;
            margin-right: 1.5rem;
            flex-shrink: 0;
        }

        .step-icon.completed {
            background: var(--primary-color);
            color: #fff;
        }

        .step-icon.active {
            background: var(--primary-color);
            color: #fff;
            animation: pulse 2s infinite;
        }

        .step-icon.pending {
            background: var(--bg-light);
            color: var(--text-light);
            border: 2px solid var(--border-color);
        }

        .step-content {
            flex: 1;
        }

        .step-content h4 {
            margin: 0 0 0.25rem 0;
            color: var(--text-dark);
        }

        .step-content p {
            margin: 0;
            color: var(--text-light);
            font-size: 0.9rem;
        }

        .step-line {
            position: absolute;
            left: 25px;
            top: 50px;
            width: 2px;
            height: calc(100% - 50px);
            background: var(--border-color);
        }

        .step-line.completed {
            background: var(--primary-color);
        }

        @keyframes pulse {
            0%, 100% { opacity: 1; }
            50% { opacity: 0.7; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="tracking-container">
            <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
                <div style="text-align: center; padding: 4rem;">
                    <h2>Commande non trouvée</h2>
                    <p style="color: var(--text-light); margin-bottom: 2rem;">La commande que vous recherchez n'existe pas.</p>
                    <a href="Profile.aspx" class="btn btn-primary">Retour à mon compte</a>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlTracking" runat="server">
                <div class="tracking-header">
                    <h1 style="margin: 0 0 0.5rem 0;">Suivi de la commande</h1>
                    <p style="margin: 0; color: var(--text-light); font-size: 1.1rem;">
                        Commande <asp:Label ID="lblOrderNumber" runat="server"></asp:Label>
                    </p>
                </div>

                <div class="card">
                    <div class="tracking-steps">
                        <asp:Repeater ID="rptTrackingSteps" runat="server">
                            <ItemTemplate>
                                <div class="tracking-step">
                                    <div class='step-icon <%= GetStepClass(Container.DataItem, Container.ItemIndex) %>'>
                                        <%= GetStepIcon(Container.ItemIndex) %>
                                    </div>
                                    <div class="step-content">
                                        <h4><%# GetStepTitle(Container.DataItem) %></h4>
                                        <p><%# GetStepDescription(Container.DataItem) %></p>
                                        <%# GetStepDate(Container.DataItem) %>
                                    </div>
                                    <%# Container.ItemIndex < GetTotalSteps() - 1 ? "<div class='step-line " + GetStepLineClass(Container.DataItem, Container.ItemIndex) + "'></div>" : "" %>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <div style="margin-top: 2rem; text-align: center;">
                    <a href='OrderDetails.aspx?id=<%= Request.QueryString["id"] %>' class="btn btn-outline">
                        <i class="fas fa-info-circle"></i> Voir les détails de la commande
                    </a>
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>


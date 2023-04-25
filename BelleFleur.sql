-- Table of content
-- 1. Init
-- 2. Schema
-- 3. Triggers
-- 4. Inserts
-- 5. Procedures

-- 
-- INIT
--
DROP DATABASE IF EXISTS BelleFleur;
CREATE DATABASE BelleFleur;
USE BelleFleur;

-- 
-- SCHEMA
-- 

CREATE TABLE client (
    client_id int NOT NULL PRIMARY KEY,
    client_prenom text,
    client_nom text,
    client_email varchar(50),
    client_adresse text,
    client_carte_de_credit text,
    client_pass text
);

CREATE TABLE reduction (
    reduction_id int NOT NULL PRIMARY KEY,
    reduction_nom varchar(50), 
    reduction_commandes_mois int, 
    reduction_valeur decimal
);

CREATE TABLE magasin (
    magasin_id int NOT NULL PRIMARY KEY,
    magasin_nom text,
    magasin_adresse text
);

CREATE TABLE bouquet (
    bouquet_id int NOT NULL PRIMARY KEY,
    bouquet_nom text,
    bouquet_prix decimal,
    bouquet_description text,
    bouquet_categorie text
);

CREATE TABLE commande (
    commande_id int NOT NULL PRIMARY KEY,
    commande_adresse_livraison text,
    commande_date_livraison_desiree datetime,
    commande_etat varchar(10),
    commande_date_creation datetime,
    commande_prix decimal,
    commande_reduction decimal,
    client_id int,
    bouquet_id text,
    magasin_id int,
    reduction_id int,
    FOREIGN KEY (client_id) REFERENCES client (client_id),
    FOREIGN KEY (bouquet_id) REFERENCES bouquet (bouquet_id),
    FOREIGN KEY (magasin_id) REFERENCES magasin (magasin_id),
    FOREIGN KEY (reduction_id) REFERENCES reduction (reduction_id)
);

CREATE TABLE produit (
    produit_id int NOT NULL PRIMARY KEY,
    produit_nom text,
    produit_type varchar(15),
    produit_prix decimal,
    produit_categorie enum('Fleur Classique', 'Fleur exotique', 'Accessoire'),
    produit_disponibilite_mois text, -- "1,2,3,9,10,11,12" / "*"
);

CREATE TABLE employe (
    employe_id int NOT NULL PRIMARY KEY,
    employe_proprietaire bool,
    employe_email varchar(50),
    employe_pass text,
    employe_prenom text,
    employe_nom text,
    magasin_id int,
    FOREIGN KEY (magasin_id) REFERENCES magasin (magasin_id)
);

CREATE TABLE compositionbouquet (
    bouquet_id int NOT NULL,
    produit_id int NOT NULL,
    composition_quantite int,
    PRIMARY KEY (bouquet_id, produit_id),
    FOREIGN KEY (bouquet_id) REFERENCES bouquet (bouquet_id),
    FOREIGN KEY (produit_id) REFERENCES produit (produit_id)
);

CREATE TABLE stocks (
    magasin_id int NOT NULL,
    produit_id int NOT NULL,
    stock_qte int,
    stock_qte_minimum int,
    PRIMARY KEY (magasin_id, produit_id),
    FOREIGN KEY (magasin_id) REFERENCES magasin (magasin_id),
    FOREIGN KEY (produit_id) REFERENCES produit (produit_id)
);

-- 
-- TRIGGERS
--

-- Trigger 1: After a new magasin is created
CREATE TRIGGER insert_zero_stock_for_new_magasin
AFTER INSERT
ON magasin FOR EACH ROW
BEGIN
    INSERT INTO stocks (magasin_id, produit_id, stock_qte, stock_qte_minimum)
    SELECT NEW.magasin_id, produit.produit_id, 0, 0
    FROM produit;
END;

-- Trigger 2: After a new produit is created
CREATE TRIGGER insert_zero_stock_for_new_produit
AFTER INSERT
ON produit FOR EACH ROW
BEGIN
    INSERT INTO stocks (magasin_id, produit_id, stock_qte, stock_qte_minimum)
    SELECT magasin.magasin_id, NEW.produit_id, 0, 0
    FROM magasin;
END;

-- 
-- INSERTS
--


-- Bouquets/Produits/Compositions
INSERT INTO bouquet (bouquet_id, bouquet_nom, bouquet_prix, bouquet_description, bouquet_categorie) VALUES
(1, 'Gros Merci', 45, 'Arrangement floral avec marguerites et verdure', 'Toute occasion'),
(2, 'L’amoureux', 65, 'Arrangement floral avec roses blanches et roses rouges', 'St-Valentin'),
(3, 'L’Exotique', 40, 'Arrangement floral avec ginger, oiseaux du paradis, roses et genet', 'Toute occasion'),
(4, 'Maman', 80, 'Arrangement floral avec gerbera, roses blanches, lys et alstroméria', 'Fête des mères'),
(5, 'Vive la mariée', 120, 'Arrangement floral avec lys et orchidées', 'Mariage');

INSERT INTO produit (produit_id, produit_nom, produit_type, produit_prix, produit_categorie, produit_disponibilite_mois) VALUES
(1, 'Gerbera', 'fleur', 5.00, 'Fleur Classique', '*'),
(2, 'Ginger', 'fleur', 4.00, 'Fleur exotique', '*'),
(3, 'Glaïeul', 'fleur', 1.00, 'Fleur exotique', '5,6,7,8,9,10'),
(4, 'Marguerite', 'fleur', 2.25, 'Fleur Classique', '*'),
(5, 'Rose rouge', 'fleur', 2.50, 'Fleur Classique', '*'),
(6, 'Lys', 'fleur', 3.50, 'Fleur Classique', '*'),
(7, 'Orchidée', 'fleur', 8.00, 'Fleur exotique', '*'),
(8, 'Verdure', 'accessoire', 2.00, 'Fleur Classique', '*'),
(9, 'Oiseau du paradis', 'fleur', 6.00, 'Fleur exotique', '*');

INSERT INTO compositionbouquet (bouquet_id, produit_id, composition_quantite) VALUES
(1, 4, 3), -- Gros Merci avec marguerites
(1, 8, 2), -- Gros Merci avec verdure
(2, 5, 6), -- L'amoureux avec roses rouges
(2, 5, 4), -- L'amoureux avec roses blanches
(3, 2, 3), -- L'Exotique avec ginger
(3, 4, 4), -- L'Exotique avec marguerites
(3, 9, 2), -- L'Exotique avec oiseaux du paradis
(3, 5, 3), -- L'Exotique avec roses
(4, 1, 3), -- Maman avec gerberas
(4, 5, 2), -- Maman avec roses blanches
(4, 6, 1), -- Maman avec lys
(4, 7, 2), -- Maman avec orchidées
(5, 1, 5), -- Vive la mariée avec lys
(5, 7, 3); -- Vive la mariée avec orchidées


-- Réductions
INSERT INTO reduction(reduction_id, reduction_nom, reduction_commandes_mois, reduction_valeur)
VALUES
(1, 'Fidélité - Normal', 0, 0.0),
(2, 'Fidélité - Bronze', 1, 0.05),
(3, 'Fidélité - OR', 1, 0.15);

-- Magasins/employés


--
-- PROCEDURES
-- 

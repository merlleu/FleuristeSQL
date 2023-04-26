-- Table of content
-- 1. Init
-- 2. Schema
-- 3. Triggers/Procedures
-- 4. Inserts

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
    client_id int NOT NULL PRIMARY KEY AUTO_INCREMENT,
    client_prenom VARCHAR(50),
    client_nom VARCHAR(50),
    client_email varchar(50),
    client_adresse VARCHAR(255),
    client_carte_de_credit VARCHAR(50),
    client_pass VARCHAR(200)
);

CREATE TABLE reduction (
    reduction_id int NOT NULL PRIMARY KEY AUTO_INCREMENT,
    reduction_nom varchar(50), 
    reduction_commandes_mois int, 
    reduction_valeur decimal
);

CREATE TABLE magasin (
    magasin_id int NOT NULL PRIMARY KEY AUTO_INCREMENT,
    magasin_nom VARCHAR(50),
    magasin_adresse VARCHAR(255)
);

CREATE TABLE bouquet (
    bouquet_id int NOT NULL PRIMARY KEY AUTO_INCREMENT,
    bouquet_nom VARCHAR(50),
    bouquet_prix decimal,
    bouquet_description VARCHAR(255),
    bouquet_categorie ENUM('Toute occasion', 'St-Valentin', 'Fête des mères', 'Mariage', 'Personnalisé')
);

CREATE TABLE commande (
    commande_id int NOT NULL PRIMARY KEY AUTO_INCREMENT,
    commande_adresse_livraison VARCHAR(255),
    commande_date_livraison_desiree datetime,
    commande_etat ENUM('VINV', 'CC', 'CPAV', 'CAL', 'CL'),
    commande_date_creation datetime,
    client_id int,
    bouquet_id int,
    magasin_id int,
    reduction_id int,
    FOREIGN KEY (client_id) REFERENCES client (client_id),
    FOREIGN KEY (bouquet_id) REFERENCES bouquet (bouquet_id),
    FOREIGN KEY (magasin_id) REFERENCES magasin (magasin_id),
    FOREIGN KEY (reduction_id) REFERENCES reduction (reduction_id)
);

CREATE TABLE produit (
    produit_id int NOT NULL PRIMARY KEY AUTO_INCREMENT,
    produit_nom varchar(50),
    produit_type ENUM('fleur', 'accessoire'),
    produit_prix decimal,
    produit_categorie ENUM('Fleur Classique', 'Fleur exotique', 'Accessoire'),
    produit_disponibilite_mois VARCHAR(50) -- "1,2,3,9,10,11,12" / "*"
);

CREATE TABLE employe (
    employe_id int NOT NULL PRIMARY KEY AUTO_INCREMENT,
    employe_proprietaire bool,
    employe_email varchar(50),
    employe_pass VARCHAR(200),
    employe_prenom varchar(50),
    employe_nom varchar(50),
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
-- TRIGGERS/PROCEDURES
--

-- On change le délimiteur pour pouvoir utiliser le ; dans les procédures
DELIMITER $$


-- Procedure 1: Récupère la réduction en fonction du nombre de commandes du client le mois précédent
CREATE PROCEDURE get_reduction_client(client_id_param INT)
BEGIN
    -- Variables
    DECLARE commandes INT;
    -- Compter le nombre de commandes du client le mois précédent
    SELECT COUNT(*) AS commandes
    FROM commande
    WHERE client_id = client_id_param
        AND commande_date_creation >= DATE_SUB(NOW(), INTERVAL 30 DAY);
        
    -- Trouver la meilleure réduction disponible pour le nombre de commandes
    SELECT *
    FROM reduction
    WHERE reduction_commandes_mois <= commandes
    ORDER BY reduction_commandes_mois DESC
    LIMIT 1;
END$$



-- Trigger 1: Creer un stock pour un nouveau magasin
CREATE TRIGGER insert_zero_stock_for_new_magasin
AFTER INSERT
ON magasin 
FOR EACH ROW
BEGIN
    INSERT INTO stocks (magasin_id, produit_id, stock_qte, stock_qte_minimum)
    SELECT NEW.magasin_id, produit.produit_id, 0, 0
    FROM produit;
END$$

-- Trigger 2: Creer un stock pour un nouveau produit
CREATE TRIGGER insert_zero_stock_for_new_produit
AFTER INSERT
ON produit 
FOR EACH ROW
BEGIN
    INSERT INTO stocks (magasin_id, produit_id, stock_qte, stock_qte_minimum)
    SELECT magasin.magasin_id, NEW.produit_id, 0, 0
    FROM magasin;
END$$

-- Trigger 3: Mettre à jour les stocks lors du passage d'une commande au statut "CC"
CREATE TRIGGER update_stock
AFTER UPDATE 
ON commande
FOR EACH ROW
BEGIN
  IF NEW.commande_etat = 'CC' THEN
    UPDATE stocks
    SET stock_qte = stock_qte - (
            SELECT composition_quantite 
            FROM compositionbouquet 
            WHERE bouquet_id = NEW.bouquet_id AND produit_id = produit_id
        )
    WHERE produit_id IN (SELECT produit_id FROM compositionbouquet WHERE bouquet_id = NEW.bouquet_id) AND magasin_id = NEW.magasin_id;
  END IF;
END$$

DELIMITER ;

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
INSERT INTO magasin (magasin_id, magasin_nom, magasin_adresse) VALUES
(1, 'Michel et une Belle de nuit', '5 Rue des Roses, 75001 Paris'),
(2, 'Le Pétale Rieur', '20 Rue des Lilas, 69002 Lyon'),
(3, 'La Fleur qui Fait Plaisir', '10 Rue des Iris, 33000 Bordeaux');

INSERT INTO employe (employe_id, employe_proprietaire, employe_email, employe_pass, employe_prenom, employe_nom, magasin_id) VALUES
(1, true, 'michel@bellefleur.fr', 'fleur', 'Michel', 'Bellefleur', 1),
(2, false, 'julien@bellefleur.fr', 'fleur', 'Julien', 'Leroy', 1),
(3, false, 'marie@bellefleur.fr', 'fleur', 'Marie', 'Dubois', 1),
(4, false, 'antoine@bellefleur.fr', 'fleur', 'Antoine', 'Dupont', 2),
(5, false, 'claire@bellefleur.fr', 'fleur', 'Claire', 'Martin', 2),
(6, false, 'mathieu@bellefleur.fr', 'fleur', 'Mathieu', 'Girard', 2),
(7, false, 'olivia@bellefleur.fr', 'fleur', 'Olivia', 'Rousseau', 3);

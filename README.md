> API .Net Core 2.1 avec OData
# Filtrage, pagination et recherche avec OData

> Visible OData n'est pas encore disponible en core 3.0 (13/12/2019)

## Création du projet

* Dans le menu **Fichier**, sélectionnez **Nouveau** > **Projet**.
* Sélectionnez le modèle **Application web ASP.NET Core** et cliquez sur **Suivant**.
* Nommez le projet *OData* et cliquez sur **Créer**.
* Dans la boîte de dialogue **créer une application API** , vérifiez que **.net Core** et **ASP.net Core 2.1** sont sélectionnés. Sélectionnez le modèle **API** et cliquez sur **Créer**.

### Ajout du package OData

- Dans le menu **Outils**, sélectionnez **Gestionnaire de package NuGet > Gérer les packages NuGet pour la solution**.
- Sélectionnez l’onglet **Parcourir**, puis entrez **Microsoft.AspNetCore.OData** dans la zone de recherche.
- Dans le volet gauche, sélectionnez **Microsoft.AspNetCore.OData**
- Cochez la case **Projet** dans le volet droit, puis sélectionnez **Installer**.

## Création du model

- Dans l’**Explorateur de solutions**, cliquez avec le bouton droit sur le projet. Sélectionnez **Ajouter** > **Nouveau dossier**. Nommez le dossier *Models*.

- Cliquez avec le bouton droit sur le dossier *Models* et sélectionnez **Ajouter** > **Classe**. Nommez la classe *Product* et sélectionnez sur **Ajouter**.

- Remplacez le code du modèle par le code suivant :

  ```c#
  namespace OData.Models
  {
      public class Product
      {
          public long Id { get; set; }
          public string Name { get; set; }
      }
  }
  ```

## Création du contrôleur

* Créez un nouveau contrôleur **Products**
* Faire hériter le contrôleur de **ODataController**
* Ajouter l'annotation **[EnableQuery]** pour la prise en charge OData

```c#
namespace OData.Controllers
{
    public class ProductsController : ODataController
    {
        // Initialisation in memory
        private List<Product> products = new List<Product>()
        {
            new Product()
            {
                Id = 1,
                Name = "Bread",
            },
            new Product()
            {
                Id = 1,
                Name = "Milk",
            },
            new Product()
            {
                Id = 1,
                Name = "Potatoes",
            }
        };

        [EnableQuery] // Annotation OData
        public List<Product> Get()
        {
            return products;
        }
    }
}
```

## Configuration du endpoint OData

```c#
namespace OData
{
    public class Startup
    {
		...
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddOData();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            
            var builder = new ODataConventionModelBuilder(app.ApplicationServices);
            builder.EntitySet<Product>("Products");
            app.UseMvc(routeBuilder =>
            {
                routeBuilder.EnableDependencyInjection();
                // and this line to enable OData query option, for example $filter
                routeBuilder.Select().Expand().Filter().OrderBy().Count();
                routeBuilder.MapODataServiceRoute("ODataRoute", "odata", builder.GetEdmModel());
            });
        }
    }
}

```

* Connexion à l'API via 2 url :
  * OData :  https://localhost:44337/odata/products
  * API standard : https://localhost:44337/api/products

## Utilisation d'OData

### Option de requête

| Option       | Description                                                  |
| ------------ | ------------------------------------------------------------ |
| $expand      | Développe les entités connexes inline.                       |
| $filter      | Filtre les résultats selon une condition booléenne.          |
| $inlinecount | Indique au serveur d’inclure le nombre total d’entités correspondantes dans la réponse. (Utile pour la pagination côté serveur). |
| $orderby     | Trie les résultats.                                          |
| $select      | Sélectionne les propriétés à inclure dans la réponse.        |
| $skip        | Ignore les n premiers résultats.                             |
| $top         | Retourne uniquement le n premiers résultats.                 |

### Pagination

#### Pagination client

* Utilisation des options **$top** et **$skip**
  * exemple : http://localhost/Products?$top=10&$skip=20

#### Pagination serveur

* Utilisation de l'annotation **[EnableQuery(PageSize = 2)]** sur l'action
* Génération automatique du lien permettant l'accès à la page suivante dans la réponse **@odata.nextLink**
* Pour connaitre le nombre total de résultat : **$inlinecount=allpages**

### Filtrage

* Utilisation de **$filter**

  * exemple : 

    | Description                                                  | Url                                                          |
    | ------------------------------------------------------------ | ------------------------------------------------------------ |
    | Retourne tous les produits de catégorie est égal à « Toys ». | `http://localhost/Products?$filter=Category` EQ 'Toys'       |
    | Retourner tous les produits dont le prix est inférieur à 10. | `http://localhost/Products?$filter=Price` lt 10              |
    | Fonctions de chaîne : Renvoie tous les produits avec « zz » dans leur nom. | `http://localhost/Products?$filter=substringof('zz',Name)`   |
    | Fonctions de date : Retourner tous les produits avec ReleaseDate après 2005. | `http://localhost/Products?$filter=year(ReleaseDate)` gt 2005 |

### Tri

* Utilisation de **$orderby**

  * exemple :

    | Description                                                  | Url                                                          |
    | ------------------------------------------------------------ | ------------------------------------------------------------ |
    | Trier par prix.                                              | `http://localhost/Products?$orderby=Price`                   |
    | Trier par prix décroissant (ordre décroissant).              | `http://localhost/Products?$orderby=Price desc`              |
    | Trier par catégorie, puis trier par prix en ordre décroissant dans les catégories. | `http://localhost/odata/Products?$orderby=Category,Price desc` |

## Select

* Utilisation de : 
  * **$select** : Sélectionne les propriétés indiquées.
  * **$expand** : Permet d'incluse des entités liées.
* Exemple :
  * Récupération seulement de la colonne "Name" : https://localhost:44337/odata/products?$select=name
  * Récupération des catégories avec les produits liés et les fournisseurs : http://localhost/odata/Categories?$expand=Category,Supplier
  * Récupération de donnée contenant plusieurs niveau de profondeur : http://localhost/odata/Categories$expand=Products/Supplier

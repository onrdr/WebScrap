using HtmlAgilityPack;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Web;

namespace WebScrap;

public class FetchService
{
    public static void FetchData()
    {
        var productCounter = 1;
        var pageCounter = 1;

        var coreUrl = $"https://skinsort.com";

        var productList = new List<Product>();

        var web = new HtmlWeb();

        try
        {
            while (pageCounter <= 1383)
            {
                var siteUrl = $"{coreUrl}/ingredients/page/{pageCounter}";

                var htmlDoc = web.Load(siteUrl);

                var urlList = new List<string>();

                var aTags = htmlDoc?.DocumentNode?.SelectNodes("//a[@class='w-1/2 lg:w-1/3 group cursor-pointer p-3']");

                if (aTags != null)
                {
                    foreach (var aTag in aTags)
                    {
                        var hrefValue = aTag.GetAttributeValue("href", string.Empty);
                        if (!string.IsNullOrEmpty(hrefValue))
                        {
                            urlList.Add($"{coreUrl}{hrefValue}");
                        }
                    }
                }

                if (urlList is not null && urlList.Count != 0)
                {
                    foreach (var link in urlList)
                    {
                        var product = new Product();
                        var cosIngData = new CosIngData();
                        product.CosIngData = cosIngData;
                        product.PageNumber = pageCounter;

                        htmlDoc = web.Load(link);

                        product.Name = HttpUtility.HtmlDecode(htmlDoc?.DocumentNode?.SelectSingleNode(".//article//h1")?.InnerText?.Trim());
                        Console.WriteLine($"Product {productCounter++} Name: {product.Name}");

                        var descriptionDiv = htmlDoc?.DocumentNode?.SelectSingleNode("//div[contains(@class, 'ingredient-description')]");
                        if (descriptionDiv != null)
                        {
                            var paragraphs = descriptionDiv.SelectNodes(".//p");
                            StringBuilder concatenatedText = new();

                            if (paragraphs != null)
                            {
                                foreach (var paragraph in paragraphs)
                                {
                                    concatenatedText.Append(paragraph.InnerText.Trim() + " ");
                                }
                            }

                            var explained = concatenatedText.ToString().Trim();
                            var explainedDecoded = HttpUtility.HtmlDecode(explained);
                            product.Explained = explainedDecoded;
                        }

                        var whatItIsDiv = htmlDoc?.DocumentNode?.SelectSingleNode("//div[@class='flex flex-wrap mt-1']");
                        var whatItIsNodes = whatItIsDiv?.SelectNodes(".//div[@class='px-3 text-left py-0.5']");
                        if (whatItIsNodes is not null && whatItIsNodes.Count != 0)
                        {
                            foreach (var whatItIs in whatItIsNodes)
                            {
                                product.WhatItIs.Add(HttpUtility.HtmlDecode(whatItIs.InnerText.Trim()));
                            }
                        }

                        var benefitAndConcernsDivs = htmlDoc?.DocumentNode?.SelectNodes("//div[@class='flex flex-wrap mt-2']");

                        if (benefitAndConcernsDivs is not null && benefitAndConcernsDivs.Count != 0)
                        {
                            var benefitNodes = benefitAndConcernsDivs?.FirstOrDefault()?.SelectNodes(".//div[@class='px-3 text-left py-0.5']");
                            if (benefitNodes is not null && benefitNodes.Count != 0)
                            {
                                foreach (var benefit in benefitNodes)
                                {
                                    product.Benefits.Add(HttpUtility.HtmlDecode(benefit.InnerText.Trim()));
                                }
                            }
                        }

                        if (benefitAndConcernsDivs is not null && benefitAndConcernsDivs.Count > 1)
                        {
                            var concernsNodes = benefitAndConcernsDivs?.LastOrDefault()?.SelectNodes(".//div[@class='px-3 text-left py-0.5']");
                            if (concernsNodes is not null && concernsNodes.Count != 0)
                            {
                                foreach (var concern in concernsNodes)
                                {
                                    product.Concerns.Add(HttpUtility.HtmlDecode(concern.InnerText.Trim()));
                                }
                            }
                        }

                        var whatItDoesNodes = htmlDoc?.DocumentNode?.SelectNodes("//div[contains(@class, 'border-b lg:border-none lg:mb-2 cursor-pointer')]");
                        if (whatItDoesNodes is not null && whatItDoesNodes.Count != 0)
                        {
                            foreach (var container in whatItDoesNodes)
                            {
                                var headingNode = container.SelectSingleNode(".//text()");
                                var headingText = headingNode != null ? HttpUtility.HtmlDecode(headingNode.InnerText.Trim()) : "";

                                var descriptionNode = container.SelectSingleNode(".//span[@class='text-xs text-warm-gray-700 py-2 font-normal']");
                                var descriptionText = descriptionNode != null ? HttpUtility.HtmlDecode(descriptionNode.InnerText.Trim()) : "";
                                var whatItDoes = string.Empty;

                                if (!string.IsNullOrEmpty(headingText))
                                {
                                    whatItDoes += ($"{headingText}");
                                }

                                if (!string.IsNullOrEmpty(descriptionText))
                                {
                                    whatItDoes += ($": {descriptionText}");
                                }

                                product.WhatItDoes.Add(HttpUtility.HtmlDecode(whatItDoes));
                            }
                        }

                        var alternativeNamesDiv = htmlDoc?.DocumentNode?.SelectNodes("//div[@class='border-b lg:border-none lg:mb-2 lg:text-sm lg:bg-warm-gray-100 lg:rounded-xl border-warm-gray-100 py-2.5 px-4 font-medium']");
                        if (alternativeNamesDiv is not null && alternativeNamesDiv.Count != 0)
                        {
                            foreach (var alternativeName in alternativeNamesDiv)
                            {
                                product.AlternativeNames.Add(HttpUtility.HtmlDecode(alternativeName.InnerText.Trim()));
                            }
                        }

                        var cosIngDataDiv = htmlDoc?.DocumentNode?.SelectSingleNode("//div[@class='rounded-xl flex flex-col bg-white mb-20 lg:max-w-md']");

                        cosIngData.CosIngID = HttpUtility.HtmlDecode(cosIngDataDiv?.SelectSingleNode(".//div[contains(., 'CosIng ID')]/span")?.InnerText?.Trim());
                        cosIngData.INCIName = HttpUtility.HtmlDecode(cosIngDataDiv?.SelectSingleNode(".//div[contains(., 'INCI Name')]/span")?.InnerText?.Trim());
                        cosIngData.InnName = HttpUtility.HtmlDecode(cosIngDataDiv?.SelectSingleNode(".//div[contains(., 'INN Name')]/span")?.InnerText?.Trim());
                        cosIngData.ECNumber = HttpUtility.HtmlDecode(cosIngDataDiv?.SelectSingleNode(".//div[contains(., 'EC #')]/span")?.InnerText?.Replace("&nbsp;", "")?.Trim());
                        cosIngData.PhEurName = HttpUtility.HtmlDecode(cosIngDataDiv?.SelectSingleNode(".//div[contains(., 'Ph. Eur. Name')]/span")?.InnerText?.Trim());
                        cosIngData.AllFunctions = HttpUtility.HtmlDecode(cosIngDataDiv?.SelectSingleNode(".//div[contains(., 'All Functions')]/span")?.InnerText?.Trim());

                        productList.Add(product);
                    }
                }

                pageCounter++;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception thrown in page: {pageCounter}\nMessage: {e.Message} ");
        }

        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(productList, options);
        File.WriteAllText("products.json", json);
    }

    public static void FetchData2()
    {
        var productCounter = 1;
        var pageCounter = 1;

        var coreUrl = $"https://skinsort.com";

        var productList = new List<Product2>();

        var web = new HtmlWeb();

        try
        {
            while (pageCounter <= 1588)
            {
                var siteUrl = $"{coreUrl}/products/page/{pageCounter}";

                var htmlDoc = web.Load(siteUrl);

                var urlList = new List<string>();

                var hrefs = htmlDoc?.DocumentNode?
                    .SelectNodes("//span[contains(@class, 'text-warm-gray-900')]//a[@href]")
                       .Select(a => a.GetAttributeValue("href", string.Empty));

                if (hrefs != null)
                {
                    foreach (var href in hrefs)
                    {
                        if (!string.IsNullOrEmpty(href))
                        {
                            urlList.Add($"{coreUrl}{href}");
                        }
                    }
                }                

                if (urlList is not null && urlList.Count != 0)
                {
                    foreach (var link in urlList)
                    {
                        var product = new Product2
                        {
                            PageNumber = pageCounter
                        };

                        htmlDoc = web.Load(link);

                        product.ImageUrl = HttpUtility.HtmlDecode(htmlDoc?.DocumentNode?
                            .SelectSingleNode("//img[contains(@class, 'mx-auto lg:mx-0 rounded')]")?
                            .GetAttributeValue("src", string.Empty));

                        product.Brand = HttpUtility.HtmlDecode(htmlDoc?.DocumentNode?
                            .SelectSingleNode(".//span[contains(@class, 'pb-1 text-lg')]")?.InnerText?.Trim());

                        product.Name = HttpUtility.HtmlDecode(htmlDoc?.DocumentNode?
                            .SelectSingleNode(".//h1[contains(@class, 'font-header break')]")?
                            .ChildNodes
                            .Where(n => n.NodeType == HtmlNodeType.Text && !string.IsNullOrWhiteSpace(n.InnerText))
                            .Select(n => n.InnerText.Trim())
                            .FirstOrDefault());

                        var ingredientsDivs = htmlDoc?.DocumentNode?.SelectNodes("//div[contains(@class, 'col-span-8')]");
                        if (ingredientsDivs is not null && ingredientsDivs.Count != 0)
                        {
                            foreach (var div in ingredientsDivs)
                            {
                                var ingredient = HttpUtility.HtmlDecode(div.SelectSingleNode(".//span[@class='group-data-[comparison=true]:leading-tight']")?.InnerText?.Trim());
                                if (!string.IsNullOrEmpty(ingredient))
                                {
                                    product.Ingredients.Add(ingredient);
                                }
                            }
                        }

                        var quickInfoDivs = htmlDoc?.DocumentNode?.SelectNodes("//*[contains(@class, 'flex flex-row-reverse')]");
                        if (quickInfoDivs is not null && quickInfoDivs.Count != 0)
                        {
                            foreach(var div in quickInfoDivs)
                            {
                                var info = HttpUtility.HtmlDecode(div.InnerText?.Replace("\n\n\n", " ").Trim());
                                if (!string.IsNullOrEmpty(info))
                                {
                                    product.QuickInfo.Add(info);
                                }
                            }
                        }

                        Console.WriteLine($"Product {productCounter++} Name: {product.Name}");

                        productList.Add(product);
                    }
                }

                pageCounter++;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception thrown in page: {pageCounter}\nMessage: {e.Message} ");
        }

        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(productList, options);
        File.WriteAllText("products2.json", json);
    }
}

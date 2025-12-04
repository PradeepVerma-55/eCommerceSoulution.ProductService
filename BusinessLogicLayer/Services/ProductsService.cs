using AutoMapper;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.RabbitMQ;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IValidator<ProductAddRequest> _productAddValidator;
        private readonly IValidator<ProductUpdateRequest> _productUpdateRequestValidator;
        private readonly IMapper _mapper;
        private readonly IProductsRepository _productRepository;
        private readonly IRabbitMQPublisher _rabbitMQPublisher;
        private readonly ILogger<ProductsService> _logger;

        public ProductsService(IValidator<ProductAddRequest> productAddValidator
            ,IValidator<ProductUpdateRequest> productUpdateRequestValidator
            ,IMapper mapper
            ,IProductsRepository productRepository
            ,IRabbitMQPublisher rabbitMQPublisher
            ,ILogger<ProductsService> logger
            )
        {
            this._productAddValidator = productAddValidator;
            this._productUpdateRequestValidator = productUpdateRequestValidator;
            this._mapper = mapper;
            this._productRepository = productRepository;
            this._rabbitMQPublisher = rabbitMQPublisher;
            this._logger = logger;
        }
        public async Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest)
        {
            if(productAddRequest == null)
            {
                throw new ArgumentNullException(nameof(productAddRequest));
            }

            // Validate the productAddRequest using FluentValidation

            ValidationResult validationResult = await _productAddValidator.ValidateAsync(productAddRequest);
            if (!validationResult.IsValid)
            {
              string errors =  string.Join(",",validationResult.Errors.Select(err => err.ErrorMessage));
              throw new ArgumentException($"ProductAddRequest validation failed: {errors}");
            }

            //Attempt to add product

           Product product = _mapper.Map<Product>(productAddRequest);
           Product? addedProduct = await _productRepository.AddProduct(product);

           return _mapper.Map<ProductResponse>(addedProduct);
        }

        public async Task<bool> DeleteProduct(Guid productID)
        {
            Product? existingProduct = await _productRepository.GetProductByCondition(temp => temp.ProductID == productID);

            if (existingProduct == null)
            {
                return false;
            }

            //Attempt to delete product
            bool isDeleted = await _productRepository.DeleteProduct(productID);

            //Add code for posting a message to the message queue that announces the consumers about the deleted product details

            //Publish message of product.delete

            if (isDeleted)
            {
                ProductDeletionMessage message = new ProductDeletionMessage(existingProduct.ProductID, existingProduct.ProductName);
                string routingKey = "product.delete";

                _rabbitMQPublisher.Publish(routingKey, message);
            }
            return isDeleted;
        }

        public async Task<ProductResponse?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression)
        {
            Product? product = await _productRepository.GetProductByCondition(conditionExpression);
            if (product == null)
            {
                return null;
            }

            ProductResponse productResponse = _mapper.Map<ProductResponse>(product); //Invokes ProductToProductResponseMappingProfile
            return productResponse;
        }

        public async Task<List<ProductResponse?>> GetProducts()
        {
            IEnumerable<Product?> products = await _productRepository.GetProducts();


            IEnumerable<ProductResponse?> productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products); //Invokes ProductToProductResponseMappingProfile
            return productResponses.ToList();
        }

        public async Task<List<ProductResponse?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression)
        {
            IEnumerable<Product?> products = await _productRepository.GetProductsByCondition(conditionExpression);

            IEnumerable<ProductResponse?> productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products); //Invokes ProductToProductResponseMappingProfile
            return productResponses.ToList();
        }

        public async Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productUpdateRequest)
        {
            Product? existingProduct = await _productRepository.GetProductByCondition(temp => temp.ProductID == productUpdateRequest.ProductID);

            if (existingProduct == null)
            {
                throw new ArgumentException("Invalid Product ID");
            }


            //Validate the product using Fluent Validation
            ValidationResult validationResult = await _productUpdateRequestValidator.ValidateAsync(productUpdateRequest);

            // Check the validation result
            if (!validationResult.IsValid)
            {
                string errors = string.Join(", ", validationResult.Errors.Select(temp => temp.ErrorMessage)); //Error1, Error2, ...
                throw new ArgumentException(errors);
            }


            bool isProductNameChanged = productUpdateRequest.ProductName != existingProduct.ProductName;

            //Map from ProductUpdateRequest to Product type
            Product product = _mapper.Map<Product>(productUpdateRequest); //Invokes ProductUpdateRequestToProductMappingProfile

            Product? updatedProduct = await _productRepository.UpdateProduct(product);

            //Publish product.update.name message to the exchange
            _logger.LogInformation("isProductNameChanged: {IsProductNameChanged}", isProductNameChanged);
            if (isProductNameChanged)
            {
                string routingKey = "product.update.name";
                var message = new ProductNameUpdateMessage(product.ProductID, product.ProductName);
                _rabbitMQPublisher.Publish<ProductNameUpdateMessage>(routingKey, message);
                _logger.LogInformation("Published ProductNameUpdateMessage to RabbitMQ for ProductID: {ProductID}", product.ProductID);
            }


            ProductResponse? updatedProductResponse = _mapper.Map<ProductResponse>(updatedProduct);

            return updatedProductResponse;
        }
    }
}

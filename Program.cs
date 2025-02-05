using javax.xml.transform.stream;
using javax.xml.validation;
using org.xml.sax;
using System.Reflection;
using JFile = java.io.File;

namespace Xsd11IkvmTest1
{
    internal class Program
    {
        public static SchemaFactory XercesXsd11SchemaFactory = new org.apache.xerces.jaxp.validation.XMLSchema11Factory();

        public static SchemaFactory XercesXsd10SchemaFactory = new org.apache.xerces.jaxp.validation.XMLSchemaFactory();
        static void Main(string[] args)
        {
            //ikvm.runtime.Startup.addBootClassPathAssembly(Assembly.Load("xercesImpl"));

            //ikvm.runtime.Startup.addBootClassPathAssembly(Assembly.Load("java-cup"));

            //ikvm.runtime.Startup.addBootClassPathAssembly(Assembly.Load("xpath2"));
            
            //Assembly.Load("xercesImpl");
            //Assembly.Load("java-cup");
            Assembly.Load("xpath2");

            var validationResult = ValidateXmlAgainstXsd11("sample1.xml", "sample1.xsd");

            if (validationResult.Success)
            {
                Console.WriteLine(validationResult.Result);
            }
            else
            {
                Console.WriteLine(validationResult.Errors);
            }

        }

        public static ValidationResult ValidateXmlAgainstXsd11(string xmlFile, string xsdFile)
        {
            return ValidationXmlAgainstXsd(xmlFile, xsdFile, XercesXsd11SchemaFactory);
        }
        public static ValidationResult ValidateXmlAgainstAssociatedXsd11(string xmlFile)
        {
            return ValidateXmlAgainstXsd11(xmlFile, null);
        }

        static ValidationResult ValidationXmlAgainstXsd(string xmlFile, string xsdFile, SchemaFactory schemaFactory)
        {
            Schema schema;

            if (xsdFile == null || xsdFile == "")
            {
                schema = schemaFactory.newSchema();
            }
            else
            {
                try
                {
                    schema = schemaFactory.newSchema(new JFile(xsdFile));
                }
                catch (SAXParseException ex)
                {
                    return new ValidationResult() { Success = false, Errors = $"Schema parsing failed: {ex.Message}" };
                }
            }

            Validator validator = schema.newValidator();

            var myErrorHandler = new MyErrorHandler();

            validator.setErrorHandler(myErrorHandler);

            try
            {
                validator.validate(new StreamSource(new JFile(xmlFile)));
            }
            catch (SAXParseException ex)
            {
                return new ValidationResult() { Success = false, Errors = $"Validation failed\r\n: {ex.Message} ({ex.getLineNumber()}:{ex.getColumnNumber()})" };
            }

            if (myErrorHandler.Valid)
            {
                return new ValidationResult() { Success = true, Result = "XML input " + xmlFile + " is valid" + (!string.IsNullOrEmpty(xsdFile) ? " against schema " + xsdFile : "") + "." };
            }
            else
            {
                return new ValidationResult() { Success = false, Errors = "Validation failed:\r\n" + string.Join("\r\n", myErrorHandler.ErrorList) };
            }
        }
    }
}

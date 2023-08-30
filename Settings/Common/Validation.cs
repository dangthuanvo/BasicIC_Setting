using Common.Interfaces;
using BasicIC_Setting.KafkaComponents;
using BasicIC_Setting.Models.Common;
using BasicIC_Setting.Models.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BasicIC_Setting.Common
{
    public class Validation
    {
        private static ILogger _logger = CommonFunc.CreateInstanceDJ<ILogger>();

        public async static Task<ValidateState> ValidateModel(Object objectInstance, string topic = "")
        {
            var context = new ValidationContext(objectInstance, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            var validateState = new ValidateState();

            if (!Validator.TryValidateObject(objectInstance, context, validationResults, true))
            {
                string mess = "Model is not valid because " + string.Join(", ", validationResults.Select(s => s.ErrorMessage).ToArray());
                _logger.LogError(mess);
                validateState.mess = mess;
                validateState.status = false;

                //send log kafka
                LogMessModel logMess = new LogMessModel(topic, mess);
                ProducerWrapper<LogMessModel> _producer = new ProducerWrapper<LogMessModel>();
                await _producer.CreateMess(Topic.LOG_INVALID_DATA, logMess);
            };

            return validateState;
        }
    }
}
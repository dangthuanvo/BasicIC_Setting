using AutoMapper;
using BasicIC_Setting.Common;
using BasicIC_Setting.Interfaces;
using BasicIC_Setting.Models.Main.M02;
using BasicIC_Setting.Services.Interfaces;
using Common;
using Common.Commons;
using Common.Interfaces;
using Common.Params.Base;
using Repository.CustomModel;
using Repository.EF;
using Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace BasicIC_Setting.Services.Implement
{
    public class EmailSettingService : BaseService, IEmailSettingService
    {
        protected SettingsRepository<M02_EmailSettings> _repo;
        public EmailSettingService(SettingsRepository<M02_EmailSettings> repo,
            ILogger logger, IConfigManager config, IMapper mapper) : base(config, logger, mapper)
        {
            _repo = repo;
        }

        /// <summary>
        /// Type: Implement method
        /// Description: Method to create email setting and return data to API Create
        /// Owner: trint
        /// Create log:     15.11.2022 - trint     
        /// </summary>
        /// <param name="param"></param>      
        /// <param name="dbContext"></param>        
        /// <returns></returns>
        public async Task<ResponseService<EmailSettingModel>> Create(EmailSettingModel param, M02_BasicEntities dbContext = null)
        {
            _logger.LogInfo(GetMethodName(new System.Diagnostics.StackTrace()));

            using ((dbContext == null) ? dbContext = new M02_BasicEntities() : null)
            {
                DbContextTransaction transaction = dbContext.Database.CurrentTransaction;
                // inTransaction is when transaction already started in other services
                bool inTransaction = transaction != null ? true : false;
                if (!inTransaction)
                    transaction = dbContext.Database.BeginTransaction();

                // Equivalent to "using ", disposes transaction if it's the container (largest) transaction
                try
                {
                    // Create query parameter
                    PagingParam queyrParam = new PagingParam();

                    // Set tenant_id
                    String tenant_id = SessionStore.Get(Constants.KEY_SESSION_TENANT_ID);
                    if (!String.IsNullOrEmpty(tenant_id))
                        queyrParam.tenant_id = tenant_id;

                    // Get result from Entity
                    ListResult<M02_EmailSettings> resultEntity = await _repo.GetAll(queyrParam, dbContext);

                    // Only one email setting for one tenant
                    if (resultEntity.total > 0)
                    {
                        throw new InternalServiceException("Email setting already exists", ErrorCodes.EMAIL_SETTING_EXIST);
                    }

                    // Encrypt password
                    string ogPass = param.pass;
                    param.pass = HashHandle.Encrypt(param.pass);

                    // Create Email Setting
                    param.AddInfo();
                    M02_EmailSettings emailSettingEntity;
                    try
                    {
                        emailSettingEntity = _mapper.Map<EmailSettingModel, M02_EmailSettings>(param);
                    }
                    catch
                    {
                        throw new InternalServiceException("Error mapping EmailSettingModel to M02_EmailSetting", ErrorCodes.ERROR_MAPPING_MODELS);
                    }
                    M02_EmailSettings createResult = await _repo.Create(emailSettingEntity, dbContext);

                    // Map Email Setting back into View
                    EmailSettingModel emailSettingView;
                    try
                    {
                        emailSettingView = _mapper.Map<M02_EmailSettings, EmailSettingModel>(createResult);
                        emailSettingView.pass = ogPass;
                    }
                    catch
                    {
                        throw new InternalServiceException("Error mapping M02_EmailSetting to EmailSettingModel", ErrorCodes.ERROR_MAPPING_MODELS);
                    }

                    // Push kafka log message
                    {
                        param.pass = ogPass;

                        string createJson = Newtonsoft.Json.JsonConvert.SerializeObject(param);
                        await CommonFunc.CreateKafkaLog(param.id, Constants.LOG_USER_CREATE, "Email Setting", createJson);
                    }

                    // Commit and return
                    transaction.Commit();
                    return new ResponseService<EmailSettingModel>(emailSettingView);
                }
                catch (InternalServiceException ex)
                {
                    if (!inTransaction)
                    {
                        transaction.Rollback();
                    }
                    _logger.LogError(ex);
                    return new ResponseService<EmailSettingModel>(ex.Message).BadRequest(ex.errorCode);
                }
                catch (Exception ex)
                {
                    if (!inTransaction)
                    {
                        transaction.Rollback();
                    }
                    _logger.LogError(ex);
                    return new ResponseService<EmailSettingModel>(ex.Message).BadRequest(ErrorCodes.UNHANDLED_ERROR);
                }
                finally
                {
                    // Dispose transaction if it was started in this service
                    if (!inTransaction)
                        transaction.Dispose();
                }
            }
        }

        /// <summary>
        /// Type: Implement method
        /// Description: Method to get email setting and return data to API Get
        /// Owner: trint
        /// Create log:     15.11.2022 - trint     
        /// </summary>        
        /// <param name="dbContext"></param>        
        /// <returns></returns>
        public async Task<ResponseService<EmailSettingModel>> Get(M02_BasicEntities dbContext = null)
        {
            try
            {
                // Create query parameter
                PagingParam queyrParam = new PagingParam();

                // Set tenant_id
                String tenant_id = SessionStore.Get(Constants.KEY_SESSION_TENANT_ID);
                if (!String.IsNullOrEmpty(tenant_id))
                    queyrParam.tenant_id = tenant_id;

                // Get result from Entity
                ListResult<M02_EmailSettings> resultEntity = await _repo.GetAll(queyrParam, dbContext);

                if (resultEntity.total == 0)
                {
                    return new ResponseService<EmailSettingModel>();
                }
                else
                {
                    // Map Email Setting back into View
                    EmailSettingModel emailSettingView;
                    try
                    {
                        emailSettingView = _mapper.Map<M02_EmailSettings, EmailSettingModel>(resultEntity.items[0]);
                    }
                    catch
                    {
                        throw new InternalServiceException("Error mapping M02_EmailSetting to EmailSettingModel", ErrorCodes.ERROR_MAPPING_MODELS);
                    }

                    // Decrypt password
                    emailSettingView.pass = HashHandle.Decrypt(emailSettingView.pass);

                    return new ResponseService<EmailSettingModel>(emailSettingView);
                }
            }
            catch (InternalServiceException ex)
            {
                _logger.LogError(ex);
                return new ResponseService<EmailSettingModel>(ex.Message).BadRequest(ex.errorCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ResponseService<EmailSettingModel>(ex.Message).BadRequest(ErrorCodes.UNHANDLED_ERROR);
            }

        }

        /// <summary>
        /// Type: Implement method
        /// Description: Method to update email setting and return data to API Update
        /// Owner: trint
        /// Create log:     15.11.2022 - trint     
        /// </summary>
        /// <param name="param"></param>      
        /// <param name="dbContext"></param>        
        /// <returns></returns>
        public async Task<ResponseService<EmailSettingModel>> Update(EmailSettingModel param, M02_BasicEntities dbContext = null)
        {
            try
            {
                _logger.LogInfo(GetMethodName(new System.Diagnostics.StackTrace()));

                // Create query parameter
                PagingParam queyrParam = new PagingParam();

                // Set tenant_id for query param
                String tenant_id = SessionStore.Get(Constants.KEY_SESSION_TENANT_ID);
                if (!String.IsNullOrEmpty(tenant_id))
                    queyrParam.tenant_id = tenant_id;

                // Get result from Entity
                ListResult<M02_EmailSettings> resultEntity = await _repo.GetAll(queyrParam, dbContext);

                if (resultEntity.total == 0)
                {
                    return new ResponseService<EmailSettingModel>("RECORD_NOT_FOUND").BadRequest(ErrorCodes.RECORD_NOT_FOUND);
                }

                // Set update id to first M02_EmailSetting from db
                param.id = resultEntity.items[0].id;

                // Encrypt password
                string ogPass = param.pass;
                param.pass = HashHandle.Encrypt(param.pass);

                // Map M02_EmailSetting to EmailSettingModel then update common fields
                EmailSettingModel emailSettingEntity;
                try
                {
                    emailSettingEntity = _mapper.Map<M02_EmailSettings, EmailSettingModel>(resultEntity.items[0]);
                }
                catch
                {
                    return new ResponseService<EmailSettingModel>("Error mapping M02_EmailSetting to EmailSettingModel").BadRequest(ErrorCodes.ERROR_MAPPING_MODELS);
                }
                param.UpdateInfo(emailSettingEntity);

                // Map new updated EmailSettingModel to M02_EmailSetting then update M02_EmailSetting on db
                M02_EmailSettings emailSettingToUpdate;
                try
                {
                    emailSettingToUpdate = _mapper.Map<EmailSettingModel, M02_EmailSettings>(param, resultEntity.items[0]);
                }
                catch
                {
                    return new ResponseService<EmailSettingModel>("Error mapping EmailSettingModel to M02_EmailSetting").BadRequest(ErrorCodes.ERROR_MAPPING_MODELS);
                }
                M02_EmailSettings result = await _repo.Update(emailSettingToUpdate, dbContext);
                result.pass = ogPass;

                return new ResponseService<EmailSettingModel>(_mapper.Map<M02_EmailSettings, EmailSettingModel>(result));
            }
            catch (InternalServiceException ex)
            {
                _logger.LogError(ex);
                return new ResponseService<EmailSettingModel>(ex.Message).BadRequest(ex.errorCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ResponseService<EmailSettingModel>(ex.Message).BadRequest(ErrorCodes.UNHANDLED_ERROR);
            }
        }

        /// <summary>
        /// Type: Implement method
        /// Description: Method to delete email setting and return data to API Delete
        /// Owner: trint
        /// Create log:     15.11.2022 - trint     
        /// </summary>          
        /// <param name="dbContext"></param>        
        /// <returns></returns>
        public async Task<ResponseService<bool>> Delete(M02_BasicEntities dbContext = null)
        {
            _logger.LogInfo(GetMethodName(new System.Diagnostics.StackTrace()));

            using ((dbContext == null) ? dbContext = new M02_BasicEntities() : null)
            {
                DbContextTransaction transaction = dbContext.Database.CurrentTransaction;
                // inTransaction is when transaction already started in other services
                bool inTransaction = transaction != null ? true : false;
                if (!inTransaction)
                    transaction = dbContext.Database.BeginTransaction();

                // Equivalent to "using ", disposes transaction if it's the container (largest) transaction
                try
                {
                    // Create query parameter
                    PagingParam queyrParam = new PagingParam();

                    // Set tenant_id
                    String tenant_id = SessionStore.Get(Constants.KEY_SESSION_TENANT_ID);
                    if (!String.IsNullOrEmpty(tenant_id))
                        queyrParam.tenant_id = tenant_id;

                    // Get result from Entity
                    ListResult<M02_EmailSettings> resultEntity = await _repo.GetAll(queyrParam, dbContext);

                    // Delete
                    bool operationResult = false;
                    if (resultEntity.total > 0)
                    {
                        foreach (M02_EmailSettings emailSetting in resultEntity.items)
                        {
                            M02_EmailSettings deleteResult = await _repo.Delete(emailSetting.id, dbContext);

                            if (deleteResult == null)
                            {
                                throw new InternalServiceException("Error Deleting", ErrorCodes.ERROR_DELETING);
                            }
                        }

                        operationResult = true;
                    }

                    // Push kafka log message
                    List<EmailSettingModel> emailSettingList = _mapper.Map<List<M02_EmailSettings>, List<EmailSettingModel>>(resultEntity.items);
                    foreach (EmailSettingModel emailSetting in emailSettingList)
                    {
                        string createJson = Newtonsoft.Json.JsonConvert.SerializeObject(emailSetting);
                        await CommonFunc.CreateKafkaLog(emailSetting.id, Constants.LOG_USER_DELETE, "Email Setting", createJson);
                    }

                    // Commit and return
                    transaction.Commit();
                    return new ResponseService<bool>(operationResult);
                }
                catch (InternalServiceException ex)
                {
                    if (!inTransaction)
                    {
                        transaction.Rollback();
                    }
                    _logger.LogError(ex);
                    return new ResponseService<bool>(ex.Message).BadRequest(ex.errorCode);
                }
                catch (Exception ex)
                {
                    if (!inTransaction)
                    {
                        transaction.Rollback();
                    }
                    _logger.LogError(ex);
                    return new ResponseService<bool>(ex.Message).BadRequest(ErrorCodes.UNHANDLED_ERROR);
                }
                finally
                {
                    // Dispose transaction if it was started in this service
                    if (!inTransaction)
                        transaction.Dispose();
                }
            }
        }
    }
}
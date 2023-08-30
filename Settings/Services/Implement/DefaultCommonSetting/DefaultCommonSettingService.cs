using AutoMapper;
using BasicIC_Setting.Common;
using BasicIC_Setting.Interfaces;
using BasicIC_Setting.KafkaComponents;
using BasicIC_Setting.Models.Common;
using BasicIC_Setting.Models.Main.M02;
using BasicIC_Setting.Models.RestAPIModels;
using BasicIC_Setting.Services.Interfaces;
using Common;
using Common.ApiHelper;
using Common.Commons;
using Common.Interfaces;
using Common.Params.Base;
using Repository.CustomModel;
using Repository.EF;
using Repository.Repositories;
using Repository.SqlCommon;
using Settings.Models.Main.M02;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;

namespace Settings.Services.Implement
{
    public class DefaultCommonSettingService : DefaultBaseCRUDService<DefaultCommonSettingModel, M02_DefaultCommonSetting>, IDefaultCommonSettingService
    {
        public DefaultCommonSettingService(SettingsRepository<M02_DefaultCommonSetting> repo,
            ILogger logger, IConfigManager config, IMapper mapper) : base(repo, config, logger, mapper)
        {

        }


        public async Task<ResponseService<DefaultCommonSettingModel>> CreateCheckDuplicate(DefaultCommonSettingModel param, List<TenantModel> tenantList = null, M02_BasicEntities dbContext = null)
        {
            _logger.LogInfo(GetMethodName(new System.Diagnostics.StackTrace()));

            try
            {
                bool keyDuplicated = await CheckDuplicate(param, new string[] { "key" }, dbContext);

                // Throw exception if duplicate is found
                if (keyDuplicated)
                    throw new InternalServiceException("Default common setting's key already exists.", ErrorCodes.DEFAULT_COMMON_SETTING_KEY_EXISTS);

                // Encrypt if type is "password"
                string ogValue = param.value.ToString();
                if (param.common_type == "password")
                    param.value = HashHandle.Encrypt(param.value);

                var listParameter = new List<SqlParameter>
                    {
                        new SqlParameter("@key", param.key),
                        new SqlParameter("@value", param.value),
                        //new SqlParameter("@description", param.description),
                        new SqlParameter("@common_type", param.common_type),
                        //new SqlParameter("@only_root", param.only_root),
                        new SqlParameter("@setting_for", param.setting_for),
                        //new SqlParameter("@create_by", SessionStore.Get(Constants.KEY_SESSION_EMAIL)),
                        //new SqlParameter("@is_sync_all_tenant", param.is_sync_all_tenant)
                    }.ToArray();
                var sqlData = await SqlHelper.ExecuteStoredProcedure("spCreateDefaultCommonSetting", listParameter);
                var result = sqlData.Tables[0].ToObjectList<DefaultCommonSettingModel>();

                // create producer
                ProducerWrapper<object> _producer = new ProducerWrapper<object>();
                await _producer.CreateMess(Topic.UPDATE_DEFAULT_COMMON_SETTING, new { });

                // Update service Default Common Setting
                await CommonFunc.GetAllDefaultCommonSetting();

                return new ResponseService<DefaultCommonSettingModel>(result[0]);
            }
            catch (InternalServiceException ex)
            {
                _logger.LogError(ex);
                return new ResponseService<DefaultCommonSettingModel>(ex.Message).BadRequest(ex.errorCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ResponseService<DefaultCommonSettingModel>(ex.Message).BadRequest(ErrorCodes.UNHANDLED_ERROR);
            }
        }
        public async Task<ResponseService<ListResult<DefaultCommonSettingModel>>> GetAllDecrypted(PagingParam param, M02_BasicEntities dbContext = null)
        {
            try
            {
                _logger.LogInfo(GetMethodName(new System.Diagnostics.StackTrace()));

                // Intercept to searching value
                if (param.search_list.Count > 0)
                {
                    SearchParam searchParam = param.search_list.Find(m => m.name_field == nameof(CommonSettingModel.value));

                    if (searchParam != default)
                    {
                        string value = searchParam.value_search.ToString();
                        int valueSearch = 2;
                        if (value == "t"
                         || value == "tr"
                         || value == "tru"
                         || value == "true")
                        {
                            valueSearch = 1;
                        }
                        else if (value == "f"
                              || value == "fa"
                              || value == "fal"
                              || value == "fals"
                              || value == "false")
                        {
                            valueSearch = 0;
                        }

                        if (valueSearch == 1 || valueSearch == 0)
                        {
                            // Open parenthesis
                            searchParam.is_open_parenthesis = 1;

                            // Add search OR for (value = 0/1 AND common_type = "boolean")
                            SearchParam searchForValue = new SearchParam()
                            {
                                name_field = nameof(CommonSettingModel.value),
                                value_search = valueSearch,
                                conjunction = "OR",
                                is_open_parenthesis = 1
                            };
                            SearchParam searchForCommonType = new SearchParam()
                            {
                                name_field = nameof(CommonSettingModel.common_type),
                                value_search = "boolean",
                                conjunction = "AND",
                                is_close_parenthesis = 2
                            };

                            // Insert to after searchValue
                            int index = param.search_list.IndexOf(searchParam);
                            param.search_list.Insert(index + 1, searchForValue);
                            param.search_list.Insert(index + 2, searchForCommonType);
                        }
                    }
                }

                // Get All
                ResponseService<ListResult<DefaultCommonSettingModel>> response = await GetAll(param, dbContext);

                if (response.status
                    && response.data != null
                    && response.data.items != null
                    && response.data.items.Count > 0)
                    foreach (DefaultCommonSettingModel defaultCommonSetting in response.data.items)
                    {
                        if (defaultCommonSetting.common_type == "password")
                            defaultCommonSetting.value = HashHandle.Decrypt(defaultCommonSetting.value);
                    }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ResponseService<ListResult<DefaultCommonSettingModel>>(ex.Message).BadRequest(ErrorCodes.UNHANDLED_ERROR);
            }
        }

        /// <summary>
        /// Type: Implement method
        /// Description: Method to get default common setting (by id) and return data to API GetByIdDecrypted
        /// Owner: trint
        /// Create log:     15.11.2022 - trint     
        /// </summary>
        /// <param name="param"></param>                
        /// <returns></returns>
        public async Task<ResponseService<DefaultCommonSettingModel>> GetByIdDecrypted(ItemModel param, M02_BasicEntities dbContext = null)
        {
            try
            {
                _logger.LogInfo(GetMethodName(new System.Diagnostics.StackTrace()));

                // Get Item
                ResponseService<DefaultCommonSettingModel> response = await GetById(param, dbContext);

                // Decrypt if type is password
                if (response.status
                    && response.data != null
                    && response.data.common_type == "password")
                    response.data.value = HashHandle.Decrypt(response.data.value);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ResponseService<DefaultCommonSettingModel>(ex.Message).BadRequest(ErrorCodes.UNHANDLED_ERROR);
            }
        }

        /// <summary>
        /// Type: Implement method
        /// Description: Method to update default common setting and return data to API UpdateCheckDuplicate
        /// Owner: trint
        /// Create log:     15.11.2022 - trint     
        /// </summary>
        /// <param name="param"></param>                
        /// <returns></returns>
        public async Task<ResponseService<DefaultCommonSettingModel>> UpdateCheckDuplicate(DefaultCommonSettingModel param, M02_BasicEntities dbContext = null)
        {
            try
            {
                _logger.LogInfo(GetMethodName(new System.Diagnostics.StackTrace()));


                // Get Default Common Setting to edit
                ResponseService<DefaultCommonSettingModel> original = await GetById(new ItemModel()
                {
                    id = param.id.ToString()
                }, dbContext);

                // Throw error if getting failed
                if (!original.status)
                    throw new InternalServiceException(original.message, original.error_code);

                // Don't allow editing the key
                if (original.data.key != param.key)
                    throw new InternalServiceException("Editing Default Common Setting's key is not allowed.", ErrorCodes.NOT_ALLOWED_EDITING_DEFAULT_COMMON_SETTING_KEY);

                // Encrypt if type is "password"
                string ogValue = param.value.ToString();
                if (param.common_type == "password")
                    param.value = HashHandle.Encrypt(param.value);

                // Update
                var updateResponse = await Update(param, dbContext);

                // Push kafka log message
                if (updateResponse.Item1.status)
                {
                    if (updateResponse.Item1.data != null && updateResponse.Item1.data.value != null)
                        updateResponse.Item1.data.value = ogValue;

                    if (updateResponse.Item2 != null && updateResponse.Item2.value != null && updateResponse.Item2.common_type == "password")
                        updateResponse.Item2.value = HashHandle.Decrypt(updateResponse.Item2.value);
                }

                // create producer
                ProducerWrapper<object> _producer = new ProducerWrapper<object>();
                await _producer.CreateMess(Topic.UPDATE_DEFAULT_COMMON_SETTING, new { });

                // Update service Default Common Setting
                await CommonFunc.GetAllDefaultCommonSetting();

                return updateResponse.Item1;
            }
            catch (InternalServiceException ex)
            {
                _logger.LogError(ex);
                return new ResponseService<DefaultCommonSettingModel>(ex.Message).BadRequest(ex.errorCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ResponseService<DefaultCommonSettingModel>(ex.Message).BadRequest(ErrorCodes.UNHANDLED_ERROR);
            }
        }

        /// <summary>
        /// Type: Implement method
        /// Description: Method to delete default common setting and return data to API Delete
        /// Owner: trint
        /// Create log:     15.11.2022 - trint     
        /// </summary>
        /// <param name="param"></param>                
        /// <returns></returns>
        public override async Task<ResponseService<bool>> Delete(ItemModel param, DbContext dbContext = null, bool autoLog = true)
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
                    // Get Common Setting from db
                    ResponseService<DefaultCommonSettingModel> response = await GetById(param, dbContext);
                    if (!response.status)
                        throw new InternalServiceException("Record not found", ErrorCodes.RECORD_NOT_FOUND);

                    M02_DefaultCommonSetting result = await _repo.Delete(new Guid(param.id), dbContext);

                    // create producer
                    ProducerWrapper<object> _producer = new ProducerWrapper<object>();
                    await _producer.CreateMess(Topic.UPDATE_DEFAULT_COMMON_SETTING, new { });

                    // Update service Default Common Setting
                    await CommonFunc.GetAllDefaultCommonSetting();

                    // Save changes and commit
                    await dbContext.SaveChangesAsync();
                    if (!inTransaction)
                    {
                        transaction.Commit();
                    }

                    if (result != null)
                        return new ResponseService<bool>(true);
                    else
                        return new ResponseService<bool>(false);
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

        public async Task<bool> Test()
        {
            RestfulApi restfulApi = new RestfulApi().ToFabio("main", "");
            PagingParam paging = new PagingParam();
            var _response = await restfulApi.client.PostAsJsonAsync("customer/get-all", paging);
            var result = await _response.Content.ReadAsAsync<ResponseService<ListResult<M02_DefaultCommonSetting>>>();
            return true;
        }
    }
}
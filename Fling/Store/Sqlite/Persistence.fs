namespace Fling.Store.Sqlite.Persistence

open System
open System.Text.Json.Serialization
open Freql.Core.Common
open Freql.Sqlite

/// Module generated on 13/09/2023 17:39:40 (utc) via Freql.Tools.
[<RequireQualifiedAccess>]
module Records =
    /// A record representing a row in the table `data_blobs`.
    type DataBlob =
        { [<JsonPropertyName("id")>] Id: string
          [<JsonPropertyName("subscriptionId")>] SubscriptionId: string
          [<JsonPropertyName("rawBlob")>] RawBlob: BlobField
          [<JsonPropertyName("hash")>] Hash: string
          [<JsonPropertyName("createdOn")>] CreatedOn: DateTime
          [<JsonPropertyName("createdBy")>] CreatedBy: string }
    
        static member Blank() =
            { Id = String.Empty
              SubscriptionId = String.Empty
              RawBlob = BlobField.Empty()
              Hash = String.Empty
              CreatedOn = DateTime.UtcNow
              CreatedBy = String.Empty }
    
        static member CreateTableSql() = """
        CREATE TABLE data_blobs (
	id TEXT NOT NULL,
	subscription_id TEXT NOT NULL,
	raw_blob BLOB NOT NULL,
	hash TEXT NOT NULL,
	created_on TEXT NOT NULL,
	created_by TEXT NOT NULL,
	CONSTRAINT data_blobs_PK PRIMARY KEY (id)
)
        """
    
        static member SelectSql() = """
        SELECT
              data_blobs.`id`,
              data_blobs.`subscription_id`,
              data_blobs.`raw_blob`,
              data_blobs.`hash`,
              data_blobs.`created_on`,
              data_blobs.`created_by`
        FROM data_blobs
        """
    
        static member TableName() = "data_blobs"
    
    /// A record representing a row in the table `email_attachments`.
    type EmailAttachment =
        { [<JsonPropertyName("requestId")>] RequestId: string
          [<JsonPropertyName("attachmentBlob")>] AttachmentBlob: BlobField
          [<JsonPropertyName("hash")>] Hash: string
          [<JsonPropertyName("fileName")>] FileName: string
          [<JsonPropertyName("contentType")>] ContentType: string }
    
        static member Blank() =
            { RequestId = String.Empty
              AttachmentBlob = BlobField.Empty()
              Hash = String.Empty
              FileName = String.Empty
              ContentType = String.Empty }
    
        static member CreateTableSql() = """
        CREATE TABLE email_attachments (
	request_id TEXT NOT NULL,
	attachment_blob BLOB NOT NULL,
	hash TEXT NOT NULL,
	file_name TEXT NOT NULL,
	content_type TEXT NOT NULL,
	CONSTRAINT email_attachments_FK FOREIGN KEY (request_id) REFERENCES email_requests(id)
)
        """
    
        static member SelectSql() = """
        SELECT
              email_attachments.`request_id`,
              email_attachments.`attachment_blob`,
              email_attachments.`hash`,
              email_attachments.`file_name`,
              email_attachments.`content_type`
        FROM email_attachments
        """
    
        static member TableName() = "email_attachments"
    
    /// A record representing a row in the table `email_html_content`.
    type EmailHtmlContent =
        { [<JsonPropertyName("requestId")>] RequestId: string
          [<JsonPropertyName("content")>] Content: BlobField
          [<JsonPropertyName("hash")>] Hash: string }
    
        static member Blank() =
            { RequestId = String.Empty
              Content = BlobField.Empty()
              Hash = String.Empty }
    
        static member CreateTableSql() = """
        CREATE TABLE email_html_content (
	request_id TEXT NOT NULL,
	content BLOB NOT NULL,
	hash TEXT NOT NULL,
	CONSTRAINT email_html_content_PK PRIMARY KEY (request_id),
	CONSTRAINT email_html_content_FK FOREIGN KEY (request_id) REFERENCES email_requests(id)
)
        """
    
        static member SelectSql() = """
        SELECT
              email_html_content.`request_id`,
              email_html_content.`content`,
              email_html_content.`hash`
        FROM email_html_content
        """
    
        static member TableName() = "email_html_content"
    
    /// A record representing a row in the table `email_out_queue`.
    type EmailOutQueueItem =
        { [<JsonPropertyName("requestId")>] RequestId: string }
    
        static member Blank() =
            { RequestId = String.Empty }
    
        static member CreateTableSql() = """
        CREATE TABLE email_out_queue (
	request_id TEXT NOT NULL,
	CONSTRAINT email_out_queue_PK PRIMARY KEY (request_id),
	CONSTRAINT email_out_queue_FK FOREIGN KEY (request_id) REFERENCES email_requests(id)
)
        """
    
        static member SelectSql() = """
        SELECT
              email_out_queue.`request_id`
        FROM email_out_queue
        """
    
        static member TableName() = "email_out_queue"
    
    /// A record representing a row in the table `email_plain_text_content`.
    type EmailPlainTextContent =
        { [<JsonPropertyName("requestId")>] RequestId: string
          [<JsonPropertyName("content")>] Content: BlobField
          [<JsonPropertyName("hash")>] Hash: string }
    
        static member Blank() =
            { RequestId = String.Empty
              Content = BlobField.Empty()
              Hash = String.Empty }
    
        static member CreateTableSql() = """
        CREATE TABLE email_plain_text_content (
	request_id TEXT NOT NULL,
	content BLOB NOT NULL,
	hash TEXT NOT NULL,
	CONSTRAINT email_plain_text_content_PK PRIMARY KEY (request_id),
	CONSTRAINT email_plain_text_content_FK FOREIGN KEY (request_id) REFERENCES email_requests(id)
)
        """
    
        static member SelectSql() = """
        SELECT
              email_plain_text_content.`request_id`,
              email_plain_text_content.`content`,
              email_plain_text_content.`hash`
        FROM email_plain_text_content
        """
    
        static member TableName() = "email_plain_text_content"
    
    /// A record representing a row in the table `email_poison_queue`.
    type EmailPoisonQueueItem =
        { [<JsonPropertyName("requestId")>] RequestId: string }
    
        static member Blank() =
            { RequestId = String.Empty }
    
        static member CreateTableSql() = """
        CREATE TABLE email_poison_queue (
	request_id TEXT NOT NULL,
	CONSTRAINT email_poison_queue_PK PRIMARY KEY (request_id),
	CONSTRAINT email_poison_queue_FK FOREIGN KEY (request_id) REFERENCES email_requests(id)
)
        """
    
        static member SelectSql() = """
        SELECT
              email_poison_queue.`request_id`
        FROM email_poison_queue
        """
    
        static member TableName() = "email_poison_queue"
    
    /// A record representing a row in the table `email_requests`.
    type EmailRequest =
        { [<JsonPropertyName("id")>] Id: string
          [<JsonPropertyName("subscriptionId")>] SubscriptionId: string
          [<JsonPropertyName("queuedOn")>] QueuedOn: DateTime
          [<JsonPropertyName("requestBlob")>] RequestBlob: BlobField
          [<JsonPropertyName("hash")>] Hash: string
          [<JsonPropertyName("maxRetryAttempts")>] MaxRetryAttempts: int
          [<JsonPropertyName("transactionId")>] TransactionId: string option
          [<JsonPropertyName("templateVersionId")>] TemplateVersionId: string option
          [<JsonPropertyName("dataBlobId")>] DataBlobId: string option }
    
        static member Blank() =
            { Id = String.Empty
              SubscriptionId = String.Empty
              QueuedOn = DateTime.UtcNow
              RequestBlob = BlobField.Empty()
              Hash = String.Empty
              MaxRetryAttempts = 0
              TransactionId = None
              TemplateVersionId = None
              DataBlobId = None }
    
        static member CreateTableSql() = """
        CREATE TABLE email_requests (
	id TEXT NOT NULL,
	subscription_id TEXT NOT NULL,
	queued_on TEXT NOT NULL,
	request_blob BLOB NOT NULL,
	hash TEXT NOT NULL,
	max_retry_attempts INTEGER NOT NULL,
	transaction_id TEXT,
	template_version_id TEXT,
	data_blob_id TEXT,
	CONSTRAINT email_requests_PK PRIMARY KEY (id),
	CONSTRAINT email_requests_FK FOREIGN KEY (template_version_id) REFERENCES email_template_versions(id),
	CONSTRAINT email_requests_FK_1 FOREIGN KEY (data_blob_id) REFERENCES data_blobs(id)
)
        """
    
        static member SelectSql() = """
        SELECT
              email_requests.`id`,
              email_requests.`subscription_id`,
              email_requests.`queued_on`,
              email_requests.`request_blob`,
              email_requests.`hash`,
              email_requests.`max_retry_attempts`,
              email_requests.`transaction_id`,
              email_requests.`template_version_id`,
              email_requests.`data_blob_id`
        FROM email_requests
        """
    
        static member TableName() = "email_requests"
    
    /// A record representing a row in the table `email_send_attempts`.
    type EmailSendAttempt =
        { [<JsonPropertyName("id")>] Id: string
          [<JsonPropertyName("requestId")>] RequestId: string
          [<JsonPropertyName("attemptedOn")>] AttemptedOn: DateTime
          [<JsonPropertyName("wasSuccessful")>] WasSuccessful: bool
          [<JsonPropertyName("responseBlob")>] ResponseBlob: BlobField
          [<JsonPropertyName("hash")>] Hash: string }
    
        static member Blank() =
            { Id = String.Empty
              RequestId = String.Empty
              AttemptedOn = DateTime.UtcNow
              WasSuccessful = true
              ResponseBlob = BlobField.Empty()
              Hash = String.Empty }
    
        static member CreateTableSql() = """
        CREATE TABLE email_send_attempts (
	id TEXT NOT NULL,
	request_id TEXT NOT NULL,
	attempted_on TEXT NOT NULL,
	was_successful INTEGER NOT NULL,
	response_blob BLOB NOT NULL,
	hash TEXT NOT NULL,
	CONSTRAINT email_send_attempts_PK PRIMARY KEY (id),
	CONSTRAINT email_send_attempts_FK FOREIGN KEY (request_id) REFERENCES email_requests(id)
)
        """
    
        static member SelectSql() = """
        SELECT
              email_send_attempts.`id`,
              email_send_attempts.`request_id`,
              email_send_attempts.`attempted_on`,
              email_send_attempts.`was_successful`,
              email_send_attempts.`response_blob`,
              email_send_attempts.`hash`
        FROM email_send_attempts
        """
    
        static member TableName() = "email_send_attempts"
    
    /// A record representing a row in the table `email_template_versions`.
    type EmailTemplateVersion =
        { [<JsonPropertyName("id")>] Id: string
          [<JsonPropertyName("templateId")>] TemplateId: string
          [<JsonPropertyName("version")>] Version: int
          [<JsonPropertyName("templateBlob")>] TemplateBlob: BlobField
          [<JsonPropertyName("hash")>] Hash: string
          [<JsonPropertyName("createdOn")>] CreatedOn: DateTime
          [<JsonPropertyName("createdBy")>] CreatedBy: string }
    
        static member Blank() =
            { Id = String.Empty
              TemplateId = String.Empty
              Version = 0
              TemplateBlob = BlobField.Empty()
              Hash = String.Empty
              CreatedOn = DateTime.UtcNow
              CreatedBy = String.Empty }
    
        static member CreateTableSql() = """
        CREATE TABLE email_template_versions (
	id TEXT NOT NULL,
	template_id TEXT NOT NULL,
	version INTEGER NOT NULL,
	template_blob BLOB NOT NULL,
	hash TEXT NOT NULL,
	created_on TEXT NOT NULL,
	created_by TEXT NOT NULL,
	CONSTRAINT email_template_versions_PK PRIMARY KEY (id),
	CONSTRAINT email_template_versions_UN UNIQUE (template_id,version)
	CONSTRAINT email_template_versions_FK FOREIGN KEY (template_id) REFERENCES email_templates(id)
)
        """
    
        static member SelectSql() = """
        SELECT
              email_template_versions.`id`,
              email_template_versions.`template_id`,
              email_template_versions.`version`,
              email_template_versions.`template_blob`,
              email_template_versions.`hash`,
              email_template_versions.`created_on`,
              email_template_versions.`created_by`
        FROM email_template_versions
        """
    
        static member TableName() = "email_template_versions"
    
    /// A record representing a row in the table `email_templates`.
    type EmailTemplate =
        { [<JsonPropertyName("id")>] Id: string
          [<JsonPropertyName("subscriptionId")>] SubscriptionId: string
          [<JsonPropertyName("name")>] Name: string }
    
        static member Blank() =
            { Id = String.Empty
              SubscriptionId = String.Empty
              Name = String.Empty }
    
        static member CreateTableSql() = """
        CREATE TABLE email_templates (
	id TEXT NOT NULL,
	subscription_id TEXT NOT NULL,
	name TEXT NOT NULL,
	CONSTRAINT email_templates_PK PRIMARY KEY (id),
	CONSTRAINT email_templates_UN UNIQUE (subscription_id,name)
)
        """
    
        static member SelectSql() = """
        SELECT
              email_templates.`id`,
              email_templates.`subscription_id`,
              email_templates.`name`
        FROM email_templates
        """
    
        static member TableName() = "email_templates"
    
    /// A record representing a row in the table `email_transaction_state_requests`.
    type EmailTransactionStateRequest =
        { [<JsonPropertyName("requestId")>] RequestId: string
          [<JsonPropertyName("transactionStateId")>] TransactionStateId: string }
    
        static member Blank() =
            { RequestId = String.Empty
              TransactionStateId = String.Empty }
    
        static member CreateTableSql() = """
        CREATE TABLE email_transaction_state_requests (
	request_id TEXT NOT NULL,
	transaction_state_id TEXT NOT NULL,
	CONSTRAINT email_transaction_state_requests_PK PRIMARY KEY (request_id,transaction_state_id),
	CONSTRAINT email_transaction_state_requests_FK_1 FOREIGN KEY (request_id) REFERENCES email_requests(id),
	CONSTRAINT email_transaction_state_requests_FK_2 FOREIGN KEY (transaction_state_id) REFERENCES email_transaction_states(id)
)
        """
    
        static member SelectSql() = """
        SELECT
              email_transaction_state_requests.`request_id`,
              email_transaction_state_requests.`transaction_state_id`
        FROM email_transaction_state_requests
        """
    
        static member TableName() = "email_transaction_state_requests"
    
    /// A record representing a row in the table `email_transaction_states`.
    type EmailTransactionState =
        { [<JsonPropertyName("id")>] Id: string
          [<JsonPropertyName("transactionId")>] TransactionId: string
          [<JsonPropertyName("stateTimestamp")>] StateTimestamp: DateTime
          [<JsonPropertyName("transactionStateBlob")>] TransactionStateBlob: string }
    
        static member Blank() =
            { Id = String.Empty
              TransactionId = String.Empty
              StateTimestamp = DateTime.UtcNow
              TransactionStateBlob = String.Empty }
    
        static member CreateTableSql() = """
        CREATE TABLE email_transaction_states (
	id TEXT NOT NULL,
	transaction_id TEXT NOT NULL,
	state_timestamp TEXT NOT NULL,
	transaction_state_blob TEXT NOT NULL,
	CONSTRAINT email_transaction_states_PK PRIMARY KEY (id),
	CONSTRAINT email_transaction_states_FK FOREIGN KEY (transaction_id) REFERENCES email_transactions(id)
)
        """
    
        static member SelectSql() = """
        SELECT
              email_transaction_states.`id`,
              email_transaction_states.`transaction_id`,
              email_transaction_states.`state_timestamp`,
              email_transaction_states.`transaction_state_blob`
        FROM email_transaction_states
        """
    
        static member TableName() = "email_transaction_states"
    
    /// A record representing a row in the table `email_transactions`.
    type EmailTransactions =
        { [<JsonPropertyName("id")>] Id: string
          [<JsonPropertyName("subscriptionId")>] SubscriptionId: string
          [<JsonPropertyName("transactionType")>] TransactionType: string
          [<JsonPropertyName("transactionBlob")>] TransactionBlob: BlobField }
    
        static member Blank() =
            { Id = String.Empty
              SubscriptionId = String.Empty
              TransactionType = String.Empty
              TransactionBlob = BlobField.Empty() }
    
        static member CreateTableSql() = """
        CREATE TABLE email_transactions (
	id TEXT NOT NULL,
	subscription_id TEXT NOT NULL,
	transaction_type TEXT NOT NULL,
	transaction_blob BLOB NOT NULL,
	CONSTRAINT email_transactions_PK PRIMARY KEY (id)
)
        """
    
        static member SelectSql() = """
        SELECT
              email_transactions.`id`,
              email_transactions.`subscription_id`,
              email_transactions.`transaction_type`,
              email_transactions.`transaction_blob`
        FROM email_transactions
        """
    
        static member TableName() = "email_transactions"
    

/// Module generated on 13/09/2023 17:39:40 (utc) via Freql.Tools.
[<RequireQualifiedAccess>]
module Parameters =
    /// A record representing a new row in the table `data_blobs`.
    type NewDataBlob =
        { [<JsonPropertyName("id")>] Id: string
          [<JsonPropertyName("subscriptionId")>] SubscriptionId: string
          [<JsonPropertyName("rawBlob")>] RawBlob: BlobField
          [<JsonPropertyName("hash")>] Hash: string
          [<JsonPropertyName("createdOn")>] CreatedOn: DateTime
          [<JsonPropertyName("createdBy")>] CreatedBy: string }
    
        static member Blank() =
            { Id = String.Empty
              SubscriptionId = String.Empty
              RawBlob = BlobField.Empty()
              Hash = String.Empty
              CreatedOn = DateTime.UtcNow
              CreatedBy = String.Empty }
    
    
    /// A record representing a new row in the table `email_attachments`.
    type NewEmailAttachment =
        { [<JsonPropertyName("requestId")>] RequestId: string
          [<JsonPropertyName("attachmentBlob")>] AttachmentBlob: BlobField
          [<JsonPropertyName("hash")>] Hash: string
          [<JsonPropertyName("fileName")>] FileName: string
          [<JsonPropertyName("contentType")>] ContentType: string }
    
        static member Blank() =
            { RequestId = String.Empty
              AttachmentBlob = BlobField.Empty()
              Hash = String.Empty
              FileName = String.Empty
              ContentType = String.Empty }
    
    
    /// A record representing a new row in the table `email_html_content`.
    type NewEmailHtmlContent =
        { [<JsonPropertyName("requestId")>] RequestId: string
          [<JsonPropertyName("content")>] Content: BlobField
          [<JsonPropertyName("hash")>] Hash: string }
    
        static member Blank() =
            { RequestId = String.Empty
              Content = BlobField.Empty()
              Hash = String.Empty }
    
    
    /// A record representing a new row in the table `email_out_queue`.
    type NewEmailOutQueueItem =
        { [<JsonPropertyName("requestId")>] RequestId: string }
    
        static member Blank() =
            { RequestId = String.Empty }
    
    
    /// A record representing a new row in the table `email_plain_text_content`.
    type NewEmailPlainTextContent =
        { [<JsonPropertyName("requestId")>] RequestId: string
          [<JsonPropertyName("content")>] Content: BlobField
          [<JsonPropertyName("hash")>] Hash: string }
    
        static member Blank() =
            { RequestId = String.Empty
              Content = BlobField.Empty()
              Hash = String.Empty }
    
    
    /// A record representing a new row in the table `email_poison_queue`.
    type NewEmailPoisonQueueItem =
        { [<JsonPropertyName("requestId")>] RequestId: string }
    
        static member Blank() =
            { RequestId = String.Empty }
    
    
    /// A record representing a new row in the table `email_requests`.
    type NewEmailRequest =
        { [<JsonPropertyName("id")>] Id: string
          [<JsonPropertyName("subscriptionId")>] SubscriptionId: string
          [<JsonPropertyName("queuedOn")>] QueuedOn: DateTime
          [<JsonPropertyName("requestBlob")>] RequestBlob: BlobField
          [<JsonPropertyName("hash")>] Hash: string
          [<JsonPropertyName("maxRetryAttempts")>] MaxRetryAttempts: int
          [<JsonPropertyName("transactionId")>] TransactionId: string option
          [<JsonPropertyName("templateVersionId")>] TemplateVersionId: string option
          [<JsonPropertyName("dataBlobId")>] DataBlobId: string option }
    
        static member Blank() =
            { Id = String.Empty
              SubscriptionId = String.Empty
              QueuedOn = DateTime.UtcNow
              RequestBlob = BlobField.Empty()
              Hash = String.Empty
              MaxRetryAttempts = 0
              TransactionId = None
              TemplateVersionId = None
              DataBlobId = None }
    
    
    /// A record representing a new row in the table `email_send_attempts`.
    type NewEmailSendAttempt =
        { [<JsonPropertyName("id")>] Id: string
          [<JsonPropertyName("requestId")>] RequestId: string
          [<JsonPropertyName("attemptedOn")>] AttemptedOn: DateTime
          [<JsonPropertyName("wasSuccessful")>] WasSuccessful: bool
          [<JsonPropertyName("responseBlob")>] ResponseBlob: BlobField
          [<JsonPropertyName("hash")>] Hash: string }
    
        static member Blank() =
            { Id = String.Empty
              RequestId = String.Empty
              AttemptedOn = DateTime.UtcNow
              WasSuccessful = true
              ResponseBlob = BlobField.Empty()
              Hash = String.Empty }
    
    
    /// A record representing a new row in the table `email_template_versions`.
    type NewEmailTemplateVersion =
        { [<JsonPropertyName("id")>] Id: string
          [<JsonPropertyName("templateId")>] TemplateId: string
          [<JsonPropertyName("version")>] Version: int
          [<JsonPropertyName("templateBlob")>] TemplateBlob: BlobField
          [<JsonPropertyName("hash")>] Hash: string
          [<JsonPropertyName("createdOn")>] CreatedOn: DateTime
          [<JsonPropertyName("createdBy")>] CreatedBy: string }
    
        static member Blank() =
            { Id = String.Empty
              TemplateId = String.Empty
              Version = 0
              TemplateBlob = BlobField.Empty()
              Hash = String.Empty
              CreatedOn = DateTime.UtcNow
              CreatedBy = String.Empty }
    
    
    /// A record representing a new row in the table `email_templates`.
    type NewEmailTemplate =
        { [<JsonPropertyName("id")>] Id: string
          [<JsonPropertyName("subscriptionId")>] SubscriptionId: string
          [<JsonPropertyName("name")>] Name: string }
    
        static member Blank() =
            { Id = String.Empty
              SubscriptionId = String.Empty
              Name = String.Empty }
    
    
    /// A record representing a new row in the table `email_transaction_state_requests`.
    type NewEmailTransactionStateRequest =
        { [<JsonPropertyName("requestId")>] RequestId: string
          [<JsonPropertyName("transactionStateId")>] TransactionStateId: string }
    
        static member Blank() =
            { RequestId = String.Empty
              TransactionStateId = String.Empty }
    
    
    /// A record representing a new row in the table `email_transaction_states`.
    type NewEmailTransactionState =
        { [<JsonPropertyName("id")>] Id: string
          [<JsonPropertyName("transactionId")>] TransactionId: string
          [<JsonPropertyName("stateTimestamp")>] StateTimestamp: DateTime
          [<JsonPropertyName("transactionStateBlob")>] TransactionStateBlob: string }
    
        static member Blank() =
            { Id = String.Empty
              TransactionId = String.Empty
              StateTimestamp = DateTime.UtcNow
              TransactionStateBlob = String.Empty }
    
    
    /// A record representing a new row in the table `email_transactions`.
    type NewEmailTransactions =
        { [<JsonPropertyName("id")>] Id: string
          [<JsonPropertyName("subscriptionId")>] SubscriptionId: string
          [<JsonPropertyName("transactionType")>] TransactionType: string
          [<JsonPropertyName("transactionBlob")>] TransactionBlob: BlobField }
    
        static member Blank() =
            { Id = String.Empty
              SubscriptionId = String.Empty
              TransactionType = String.Empty
              TransactionBlob = BlobField.Empty() }
    
    
/// Module generated on 13/09/2023 17:39:40 (utc) via Freql.Tools.
[<RequireQualifiedAccess>]
module Operations =

    let buildSql (lines: string list) = lines |> String.concat Environment.NewLine

    /// Select a `Records.DataBlob` from the table `data_blobs`.
    /// Internally this calls `context.SelectSingleAnon<Records.DataBlob>` and uses Records.DataBlob.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectDataBlobRecord ctx "WHERE `field` = @0" [ box `value` ]
    let selectDataBlobRecord (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.DataBlob.SelectSql() ] @ query |> buildSql
        context.SelectSingleAnon<Records.DataBlob>(sql, parameters)
    
    /// Internally this calls `context.SelectAnon<Records.DataBlob>` and uses Records.DataBlob.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectDataBlobRecords ctx "WHERE `field` = @0" [ box `value` ]
    let selectDataBlobRecords (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.DataBlob.SelectSql() ] @ query |> buildSql
        context.SelectAnon<Records.DataBlob>(sql, parameters)
    
    let insertDataBlob (context: SqliteContext) (parameters: Parameters.NewDataBlob) =
        context.Insert("data_blobs", parameters)
    
    /// Select a `Records.EmailAttachment` from the table `email_attachments`.
    /// Internally this calls `context.SelectSingleAnon<Records.EmailAttachment>` and uses Records.EmailAttachment.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailAttachmentRecord ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailAttachmentRecord (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailAttachment.SelectSql() ] @ query |> buildSql
        context.SelectSingleAnon<Records.EmailAttachment>(sql, parameters)
    
    /// Internally this calls `context.SelectAnon<Records.EmailAttachment>` and uses Records.EmailAttachment.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailAttachmentRecords ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailAttachmentRecords (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailAttachment.SelectSql() ] @ query |> buildSql
        context.SelectAnon<Records.EmailAttachment>(sql, parameters)
    
    let insertEmailAttachment (context: SqliteContext) (parameters: Parameters.NewEmailAttachment) =
        context.Insert("email_attachments", parameters)
    
    /// Select a `Records.EmailHtmlContent` from the table `email_html_content`.
    /// Internally this calls `context.SelectSingleAnon<Records.EmailHtmlContent>` and uses Records.EmailHtmlContent.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailHtmlContentRecord ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailHtmlContentRecord (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailHtmlContent.SelectSql() ] @ query |> buildSql
        context.SelectSingleAnon<Records.EmailHtmlContent>(sql, parameters)
    
    /// Internally this calls `context.SelectAnon<Records.EmailHtmlContent>` and uses Records.EmailHtmlContent.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailHtmlContentRecords ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailHtmlContentRecords (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailHtmlContent.SelectSql() ] @ query |> buildSql
        context.SelectAnon<Records.EmailHtmlContent>(sql, parameters)
    
    let insertEmailHtmlContent (context: SqliteContext) (parameters: Parameters.NewEmailHtmlContent) =
        context.Insert("email_html_content", parameters)
    
    /// Select a `Records.EmailOutQueueItem` from the table `email_out_queue`.
    /// Internally this calls `context.SelectSingleAnon<Records.EmailOutQueueItem>` and uses Records.EmailOutQueueItem.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailOutQueueItemRecord ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailOutQueueItemRecord (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailOutQueueItem.SelectSql() ] @ query |> buildSql
        context.SelectSingleAnon<Records.EmailOutQueueItem>(sql, parameters)
    
    /// Internally this calls `context.SelectAnon<Records.EmailOutQueueItem>` and uses Records.EmailOutQueueItem.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailOutQueueItemRecords ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailOutQueueItemRecords (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailOutQueueItem.SelectSql() ] @ query |> buildSql
        context.SelectAnon<Records.EmailOutQueueItem>(sql, parameters)
    
    let insertEmailOutQueueItem (context: SqliteContext) (parameters: Parameters.NewEmailOutQueueItem) =
        context.Insert("email_out_queue", parameters)
    
    /// Select a `Records.EmailPlainTextContent` from the table `email_plain_text_content`.
    /// Internally this calls `context.SelectSingleAnon<Records.EmailPlainTextContent>` and uses Records.EmailPlainTextContent.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailPlainTextContentRecord ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailPlainTextContentRecord (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailPlainTextContent.SelectSql() ] @ query |> buildSql
        context.SelectSingleAnon<Records.EmailPlainTextContent>(sql, parameters)
    
    /// Internally this calls `context.SelectAnon<Records.EmailPlainTextContent>` and uses Records.EmailPlainTextContent.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailPlainTextContentRecords ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailPlainTextContentRecords (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailPlainTextContent.SelectSql() ] @ query |> buildSql
        context.SelectAnon<Records.EmailPlainTextContent>(sql, parameters)
    
    let insertEmailPlainTextContent (context: SqliteContext) (parameters: Parameters.NewEmailPlainTextContent) =
        context.Insert("email_plain_text_content", parameters)
    
    /// Select a `Records.EmailPoisonQueueItem` from the table `email_poison_queue`.
    /// Internally this calls `context.SelectSingleAnon<Records.EmailPoisonQueueItem>` and uses Records.EmailPoisonQueueItem.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailPoisonQueueItemRecord ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailPoisonQueueItemRecord (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailPoisonQueueItem.SelectSql() ] @ query |> buildSql
        context.SelectSingleAnon<Records.EmailPoisonQueueItem>(sql, parameters)
    
    /// Internally this calls `context.SelectAnon<Records.EmailPoisonQueueItem>` and uses Records.EmailPoisonQueueItem.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailPoisonQueueItemRecords ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailPoisonQueueItemRecords (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailPoisonQueueItem.SelectSql() ] @ query |> buildSql
        context.SelectAnon<Records.EmailPoisonQueueItem>(sql, parameters)
    
    let insertEmailPoisonQueueItem (context: SqliteContext) (parameters: Parameters.NewEmailPoisonQueueItem) =
        context.Insert("email_poison_queue", parameters)
    
    /// Select a `Records.EmailRequest` from the table `email_requests`.
    /// Internally this calls `context.SelectSingleAnon<Records.EmailRequest>` and uses Records.EmailRequest.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailRequestRecord ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailRequestRecord (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailRequest.SelectSql() ] @ query |> buildSql
        context.SelectSingleAnon<Records.EmailRequest>(sql, parameters)
    
    /// Internally this calls `context.SelectAnon<Records.EmailRequest>` and uses Records.EmailRequest.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailRequestRecords ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailRequestRecords (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailRequest.SelectSql() ] @ query |> buildSql
        context.SelectAnon<Records.EmailRequest>(sql, parameters)
    
    let insertEmailRequest (context: SqliteContext) (parameters: Parameters.NewEmailRequest) =
        context.Insert("email_requests", parameters)
    
    /// Select a `Records.EmailSendAttempt` from the table `email_send_attempts`.
    /// Internally this calls `context.SelectSingleAnon<Records.EmailSendAttempt>` and uses Records.EmailSendAttempt.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailSendAttemptRecord ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailSendAttemptRecord (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailSendAttempt.SelectSql() ] @ query |> buildSql
        context.SelectSingleAnon<Records.EmailSendAttempt>(sql, parameters)
    
    /// Internally this calls `context.SelectAnon<Records.EmailSendAttempt>` and uses Records.EmailSendAttempt.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailSendAttemptRecords ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailSendAttemptRecords (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailSendAttempt.SelectSql() ] @ query |> buildSql
        context.SelectAnon<Records.EmailSendAttempt>(sql, parameters)
    
    let insertEmailSendAttempt (context: SqliteContext) (parameters: Parameters.NewEmailSendAttempt) =
        context.Insert("email_send_attempts", parameters)
    
    /// Select a `Records.EmailTemplateVersion` from the table `email_template_versions`.
    /// Internally this calls `context.SelectSingleAnon<Records.EmailTemplateVersion>` and uses Records.EmailTemplateVersion.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailTemplateVersionRecord ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailTemplateVersionRecord (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailTemplateVersion.SelectSql() ] @ query |> buildSql
        context.SelectSingleAnon<Records.EmailTemplateVersion>(sql, parameters)
    
    /// Internally this calls `context.SelectAnon<Records.EmailTemplateVersion>` and uses Records.EmailTemplateVersion.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailTemplateVersionRecords ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailTemplateVersionRecords (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailTemplateVersion.SelectSql() ] @ query |> buildSql
        context.SelectAnon<Records.EmailTemplateVersion>(sql, parameters)
    
    let insertEmailTemplateVersion (context: SqliteContext) (parameters: Parameters.NewEmailTemplateVersion) =
        context.Insert("email_template_versions", parameters)
    
    /// Select a `Records.EmailTemplate` from the table `email_templates`.
    /// Internally this calls `context.SelectSingleAnon<Records.EmailTemplate>` and uses Records.EmailTemplate.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailTemplateRecord ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailTemplateRecord (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailTemplate.SelectSql() ] @ query |> buildSql
        context.SelectSingleAnon<Records.EmailTemplate>(sql, parameters)
    
    /// Internally this calls `context.SelectAnon<Records.EmailTemplate>` and uses Records.EmailTemplate.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailTemplateRecords ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailTemplateRecords (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailTemplate.SelectSql() ] @ query |> buildSql
        context.SelectAnon<Records.EmailTemplate>(sql, parameters)
    
    let insertEmailTemplate (context: SqliteContext) (parameters: Parameters.NewEmailTemplate) =
        context.Insert("email_templates", parameters)
    
    /// Select a `Records.EmailTransactionStateRequest` from the table `email_transaction_state_requests`.
    /// Internally this calls `context.SelectSingleAnon<Records.EmailTransactionStateRequest>` and uses Records.EmailTransactionStateRequest.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailTransactionStateRequestRecord ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailTransactionStateRequestRecord (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailTransactionStateRequest.SelectSql() ] @ query |> buildSql
        context.SelectSingleAnon<Records.EmailTransactionStateRequest>(sql, parameters)
    
    /// Internally this calls `context.SelectAnon<Records.EmailTransactionStateRequest>` and uses Records.EmailTransactionStateRequest.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailTransactionStateRequestRecords ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailTransactionStateRequestRecords (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailTransactionStateRequest.SelectSql() ] @ query |> buildSql
        context.SelectAnon<Records.EmailTransactionStateRequest>(sql, parameters)
    
    let insertEmailTransactionStateRequest (context: SqliteContext) (parameters: Parameters.NewEmailTransactionStateRequest) =
        context.Insert("email_transaction_state_requests", parameters)
    
    /// Select a `Records.EmailTransactionState` from the table `email_transaction_states`.
    /// Internally this calls `context.SelectSingleAnon<Records.EmailTransactionState>` and uses Records.EmailTransactionState.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailTransactionStateRecord ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailTransactionStateRecord (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailTransactionState.SelectSql() ] @ query |> buildSql
        context.SelectSingleAnon<Records.EmailTransactionState>(sql, parameters)
    
    /// Internally this calls `context.SelectAnon<Records.EmailTransactionState>` and uses Records.EmailTransactionState.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailTransactionStateRecords ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailTransactionStateRecords (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailTransactionState.SelectSql() ] @ query |> buildSql
        context.SelectAnon<Records.EmailTransactionState>(sql, parameters)
    
    let insertEmailTransactionState (context: SqliteContext) (parameters: Parameters.NewEmailTransactionState) =
        context.Insert("email_transaction_states", parameters)
    
    /// Select a `Records.EmailTransactions` from the table `email_transactions`.
    /// Internally this calls `context.SelectSingleAnon<Records.EmailTransactions>` and uses Records.EmailTransactions.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailTransactionsRecord ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailTransactionsRecord (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailTransactions.SelectSql() ] @ query |> buildSql
        context.SelectSingleAnon<Records.EmailTransactions>(sql, parameters)
    
    /// Internally this calls `context.SelectAnon<Records.EmailTransactions>` and uses Records.EmailTransactions.SelectSql().
    /// The caller can provide extra string lines to create a query and boxed parameters.
    /// It is up to the caller to verify the sql and parameters are correct,
    /// this should be considered an internal function (not exposed in public APIs).
    /// Parameters are assigned names based on their order in 0 indexed array. For example: @0,@1,@2...
    /// Example: selectEmailTransactionsRecords ctx "WHERE `field` = @0" [ box `value` ]
    let selectEmailTransactionsRecords (context: SqliteContext) (query: string list) (parameters: obj list) =
        let sql = [ Records.EmailTransactions.SelectSql() ] @ query |> buildSql
        context.SelectAnon<Records.EmailTransactions>(sql, parameters)
    
    let insertEmailTransactions (context: SqliteContext) (parameters: Parameters.NewEmailTransactions) =
        context.Insert("email_transactions", parameters)
    
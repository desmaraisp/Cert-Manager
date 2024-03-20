import { makeApi, Zodios, type ZodiosOptions } from "@zodios/core";
import { z } from "zod";

const CertificateModel = z.object({
  isCertificateAuthority: z.boolean().optional(),
  certificateName: z.string().min(2).max(100),
  tags: z.array(z.string()).nullish(),
  certificateDescription: z.string().min(0).max(1000).nullish(),
  requirePrivateKey: z.boolean().optional(),
});
const CertificateModelWithId = z.object({
  isCertificateAuthority: z.boolean().optional(),
  certificateName: z.string().min(2).max(100),
  tags: z.array(z.string()).nullish(),
  certificateDescription: z.string().min(0).max(1000).nullish(),
  requirePrivateKey: z.boolean().optional(),
  certificateId: z.string().uuid().optional(),
});
const ProblemDetails = z
  .object({
    type: z.string().nullable(),
    title: z.string().nullable(),
    status: z.number().int().nullable(),
    detail: z.string().nullable(),
    instance: z.string().nullable(),
  })
  .partial()
  .passthrough();
const CertificateUpdateModel = z.object({
  newCertificateName: z.string().min(2).max(100),
  newTags: z.array(z.string()).nullish(),
  newCertificateDescription: z.string().min(0).max(1000).nullish(),
});
const CertificateExpirationNotification = z
  .object({
    expiringCertificateVersionId: z.string().uuid(),
    expiringCertificateId: z.string().uuid(),
    certificateExpirationTime: z.string().datetime({ offset: true }),
    certificateRenewalTime: z.string().datetime({ offset: true }).nullable(),
    certificateName: z.string().nullable(),
    certificateDescription: z.string().nullable(),
  })
  .partial();
const MuteTimingModel = z.object({
  certificateVersionId: z.string().uuid(),
  mutedUntilUtc: z.string().datetime({ offset: true }),
});
const MuteTimingModelWithId = z.object({
  certificateVersionId: z.string().uuid(),
  mutedUntilUtc: z.string().datetime({ offset: true }),
  muteTimingId: z.string().uuid().optional(),
});
const CertificateRenewalScheduleModel = z
  .object({
    subscriptionId: z.string().uuid(),
    certificateDuration: z.string(),
    certificateSubject: z.string().nullable(),
    scheduledRenewalTime: z.string().datetime({ offset: true }),
    destinationCertificateId: z.string().uuid(),
    parentCertificateId: z.string().uuid(),
  })
  .partial();
const CertificateRenewalSubscriptionModelWithId = z.object({
  certificateDuration: z.string().optional(),
  certificateSubject: z.string().min(2).max(200),
  renewXDaysBeforeExpiration: z.number().int().gte(1).lte(89).optional(),
  destinationCertificateId: z.string().uuid(),
  parentCertificateId: z.string().uuid(),
  subscriptionId: z.string().uuid().optional(),
});
const CertificateRenewalSubscriptionModel = z.object({
  certificateDuration: z.string().optional(),
  certificateSubject: z.string().min(2).max(200),
  renewXDaysBeforeExpiration: z.number().int().gte(1).lte(89).optional(),
  destinationCertificateId: z.string().uuid(),
  parentCertificateId: z.string().uuid(),
});
const UploadFormat = z.enum([
  "Pem",
  "PemWithPrivateKey",
  "PemWithEncryptedPrivateKey",
  "PfxOrCer",
  "PemWithInlinePrivateKey",
]);
const CreateCertificateVersion_Body = z
  .object({
    Files: z.array(z.instanceof(File)),
    Password: z.string().optional(),
    CertificateId: z.string().uuid(),
    Format: UploadFormat,
  })
  .passthrough();
const CertificateVersionModel = z
  .object({
    activationDate: z.string().datetime({ offset: true }),
    cn: z.string().nullable(),
    expiryDate: z.string().datetime({ offset: true }),
    issuerName: z.string().nullable(),
    thumbprint: z.string().nullable(),
    rawCertificate: z.string().nullable(),
    certificateId: z.string().uuid(),
    certificateVersionId: z.string().uuid(),
  })
  .partial();

export const schemas = {
  CertificateModel,
  CertificateModelWithId,
  ProblemDetails,
  CertificateUpdateModel,
  CertificateExpirationNotification,
  MuteTimingModel,
  MuteTimingModelWithId,
  CertificateRenewalScheduleModel,
  CertificateRenewalSubscriptionModelWithId,
  CertificateRenewalSubscriptionModel,
  UploadFormat,
  CreateCertificateVersion_Body,
  CertificateVersionModel,
};

const endpoints = makeApi([
  {
    method: "get",
    path: "/:organizationId/api/v1/CertificateRenewalSchedules",
    alias: "GetCertificateRenewalSchedules",
    requestFormat: "json",
    parameters: [
      {
        name: "MinimumUtcScheduledTime",
        type: "Query",
        schema: z.string().datetime({ offset: true }).optional(),
      },
      {
        name: "MaximumUtcScheduledTime",
        type: "Query",
        schema: z.string().datetime({ offset: true }).optional(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.array(CertificateRenewalScheduleModel),
  },
  {
    method: "get",
    path: "/:organizationId/api/v1/CertificateRenewalSubscriptions",
    alias: "GetCertificateRenewalSubscriptions",
    requestFormat: "json",
    parameters: [
      {
        name: "CertificateIds",
        type: "Query",
        schema: z.array(z.string()).optional(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.array(CertificateRenewalSubscriptionModelWithId),
  },
  {
    method: "post",
    path: "/:organizationId/api/v1/CertificateRenewalSubscriptions",
    alias: "CreateCertificateRenewalSubscriptions",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: CertificateRenewalSubscriptionModel,
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: CertificateRenewalSubscriptionModelWithId,
  },
  {
    method: "delete",
    path: "/:organizationId/api/v1/CertificateRenewalSubscriptions/:id",
    alias: "DeleteCertificateRenewalSubscription",
    requestFormat: "json",
    parameters: [
      {
        name: "id",
        type: "Path",
        schema: z.string().uuid(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.void(),
    errors: [
      {
        status: 400,
        description: `Bad Request`,
        schema: z
          .object({
            type: z.string().nullable(),
            title: z.string().nullable(),
            status: z.number().int().nullable(),
            detail: z.string().nullable(),
            instance: z.string().nullable(),
          })
          .partial()
          .passthrough(),
      },
    ],
  },
  {
    method: "post",
    path: "/:organizationId/api/v1/Certificates",
    alias: "CreateCertificate",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: CertificateModel,
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: CertificateModelWithId,
    errors: [
      {
        status: 400,
        description: `Bad Request`,
        schema: z
          .object({
            type: z.string().nullable(),
            title: z.string().nullable(),
            status: z.number().int().nullable(),
            detail: z.string().nullable(),
            instance: z.string().nullable(),
          })
          .partial()
          .passthrough(),
      },
    ],
  },
  {
    method: "get",
    path: "/:organizationId/api/v1/Certificates",
    alias: "GetAllCertificates",
    requestFormat: "json",
    parameters: [
      {
        name: "TagsToSearch",
        type: "Query",
        schema: z.array(z.string()).optional(),
      },
      {
        name: "TagsSearchBehavior",
        type: "Query",
        schema: z.enum(["MatchAll", "MatchAny"]).optional(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.array(CertificateModelWithId),
  },
  {
    method: "get",
    path: "/:organizationId/api/v1/Certificates/:id",
    alias: "GetCertificateById",
    requestFormat: "json",
    parameters: [
      {
        name: "id",
        type: "Path",
        schema: z.string().uuid(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: CertificateModelWithId,
    errors: [
      {
        status: 404,
        description: `Not Found`,
        schema: z
          .object({
            type: z.string().nullable(),
            title: z.string().nullable(),
            status: z.number().int().nullable(),
            detail: z.string().nullable(),
            instance: z.string().nullable(),
          })
          .partial()
          .passthrough(),
      },
    ],
  },
  {
    method: "delete",
    path: "/:organizationId/api/v1/Certificates/:id",
    alias: "DeleteCertificateById",
    requestFormat: "json",
    parameters: [
      {
        name: "id",
        type: "Path",
        schema: z.string().uuid(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.void(),
    errors: [
      {
        status: 404,
        description: `Not Found`,
        schema: z
          .object({
            type: z.string().nullable(),
            title: z.string().nullable(),
            status: z.number().int().nullable(),
            detail: z.string().nullable(),
            instance: z.string().nullable(),
          })
          .partial()
          .passthrough(),
      },
    ],
  },
  {
    method: "patch",
    path: "/:organizationId/api/v1/Certificates/:id",
    alias: "EditCertificateById",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: CertificateUpdateModel,
      },
      {
        name: "id",
        type: "Path",
        schema: z.string().uuid(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: CertificateModelWithId,
    errors: [
      {
        status: 404,
        description: `Not Found`,
        schema: z
          .object({
            type: z.string().nullable(),
            title: z.string().nullable(),
            status: z.number().int().nullable(),
            detail: z.string().nullable(),
            instance: z.string().nullable(),
          })
          .partial()
          .passthrough(),
      },
    ],
  },
  {
    method: "post",
    path: "/:organizationId/api/v1/CertificateVersion",
    alias: "CreateCertificateVersion",
    requestFormat: "form-data",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: CreateCertificateVersion_Body,
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: CertificateVersionModel,
    errors: [
      {
        status: 400,
        description: `Bad Request`,
        schema: z
          .object({
            type: z.string().nullable(),
            title: z.string().nullable(),
            status: z.number().int().nullable(),
            detail: z.string().nullable(),
            instance: z.string().nullable(),
          })
          .partial()
          .passthrough(),
      },
    ],
  },
  {
    method: "get",
    path: "/:organizationId/api/v1/CertificateVersions",
    alias: "GetCertificateVersions",
    requestFormat: "json",
    parameters: [
      {
        name: "CertificateIds",
        type: "Query",
        schema: z.array(z.string()).optional(),
      },
      {
        name: "MinimumUtcExpirationTime",
        type: "Query",
        schema: z.string().datetime({ offset: true }).optional(),
      },
      {
        name: "MaximumUtcExpirationTime",
        type: "Query",
        schema: z.string().datetime({ offset: true }).optional(),
      },
      {
        name: "MinimumUtcActivationTime",
        type: "Query",
        schema: z.string().datetime({ offset: true }).optional(),
      },
      {
        name: "MaximumUtcActivationTime",
        type: "Query",
        schema: z.string().datetime({ offset: true }).optional(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.array(CertificateVersionModel),
  },
  {
    method: "get",
    path: "/:organizationId/api/v1/CertificateVersions/:id",
    alias: "GetCertificateVersionById",
    requestFormat: "json",
    parameters: [
      {
        name: "id",
        type: "Path",
        schema: z.string().uuid(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: CertificateVersionModel,
    errors: [
      {
        status: 404,
        description: `Not Found`,
        schema: z
          .object({
            type: z.string().nullable(),
            title: z.string().nullable(),
            status: z.number().int().nullable(),
            detail: z.string().nullable(),
            instance: z.string().nullable(),
          })
          .partial()
          .passthrough(),
      },
    ],
  },
  {
    method: "delete",
    path: "/:organizationId/api/v1/CertificateVersions/:id",
    alias: "DeleteCertificateVersion",
    requestFormat: "json",
    parameters: [
      {
        name: "id",
        type: "Path",
        schema: z.string().uuid(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.void(),
    errors: [
      {
        status: 404,
        description: `Not Found`,
        schema: z
          .object({
            type: z.string().nullable(),
            title: z.string().nullable(),
            status: z.number().int().nullable(),
            detail: z.string().nullable(),
            instance: z.string().nullable(),
          })
          .partial()
          .passthrough(),
      },
    ],
  },
  {
    method: "get",
    path: "/:organizationId/api/v1/ExpiringCertificateVersionNotifications",
    alias: "GetExpiringCertificateVersionNotifications",
    requestFormat: "json",
    parameters: [
      {
        name: "MinimumVersionExpirationTimeUtc",
        type: "Query",
        schema: z.string().datetime({ offset: true }).optional(),
      },
      {
        name: "MaximumVersionExpirationTimeUtc",
        type: "Query",
        schema: z.string().datetime({ offset: true }).optional(),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.array(CertificateExpirationNotification),
  },
  {
    method: "patch",
    path: "/:organizationId/api/v1/MuteTimings",
    alias: "CreateMuteTimings",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.array(MuteTimingModel),
      },
      {
        name: "organizationId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.array(MuteTimingModelWithId),
  },
]);

export const api = new Zodios(endpoints);

export function createApiClient(baseUrl: string, options?: ZodiosOptions) {
  return new Zodios(baseUrl, endpoints, options);
}

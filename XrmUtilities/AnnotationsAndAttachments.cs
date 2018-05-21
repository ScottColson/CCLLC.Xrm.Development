using Microsoft.Xrm.Sdk;
using System;

namespace CCLLC.Xrm.Sdk.Utilities
{
    public class AnnotationsAndAttachments
    {
        private IOrganizationService OrganizationService { get; set; }

        public AnnotationsAndAttachments(IOrganizationService OrgService)
        {
            this.OrganizationService = OrgService;
        }


        /// <summary>
        /// Attach a note to the referenced Entity.
        /// </summary>
        /// <param name="Regarding">EntityReference of the Entity that the note will be attached to.</param>
        /// <param name="Subject">Subject line of the note</param>
        /// <param name="Text">Text of the note</param>
        public void AttachNote(EntityReference Regarding, string Subject, string Text)
        {
            Entity note = new Entity("annotation");
            note.Attributes.Add("subject", Subject);
            note.Attributes.Add("notetext", Text);
            note.Attributes.Add("objectid", Regarding);
            OrganizationService.Create(note);
        }



        /// <summary>
        /// Attach a note to the referenced Entity.
        /// </summary>
        /// <param name="EntityLogicalName">Logicl name of the Entity.</param>
        /// <param name="Id">GUID of the Entity</param>
        /// <param name="Subject">Subject line of the note.</param>
        /// <param name="Text">Text of the note.</param>
        public void AttachNote(string EntityLogicalName, Guid Id, string Subject, string Text)
        {
            EntityReference regarding = new EntityReference(EntityLogicalName, Id);
            this.AttachNote(regarding, Subject, Text);
        }


        public void UploadAttachement(EntityReference RegardingId, string FileName, string Base64DocumentBody, string MimeType)
        {
            Entity note = new Entity("annotation");
            note.Attributes.Add("subject", FileName);
            note.Attributes.Add("objectid", RegardingId);
            note.Attributes.Add("filename", FileName);
            note.Attributes.Add("documentbody", Base64DocumentBody);
            note.Attributes.Add("mimetype", MimeType);
            OrganizationService.Create(note);
        }


    }
}

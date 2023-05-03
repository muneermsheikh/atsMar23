import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { ToastrService } from 'ngx-toastr';
import { IMessage } from 'src/app/shared/models/admin/message';
import { EmailMessageSpecParams } from 'src/app/shared/params/admin/emailMessageSpecParams';
import { ConfirmService } from 'src/app/shared/services/confirm.service';
import { MessageService } from 'src/app/shared/services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
 

  @ViewChild('search', {static: false}) searchTerm: ElementRef | undefined;
  
  totalCount: number=0;
  mParams = new EmailMessageSpecParams();

  messages: IMessage[]=[];
  message: IMessage| undefined;
  
  container = 'draft';
  controlsDisabled=false;
  
  pageIndex = 1;
  pageSize = 3;
  loading = false;

  sortOptions = [
    {name:'By Message sent Asc', value:'messagesent'},
    {name:'By Message sent Desc', value:'messagesentdesc'},
    {name:'By Sender', value:'sender'},
    {name:'By Recipient', value:'recipient'},
  ]

  //months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];


  constructor(private service: MessageService, private toastr: ToastrService, private confirmService: ConfirmService) { }

  ngOnInit(): void {
    this.mParams.pageSize = this.pageSize;
    this.mParams.container =  this.container;
    this.controlsDisabled = this.mParams.container !== 'draft';
    //console.log('controlsDisabled', this.controlsDisabled);
    this.getMessages(false);
    console.log(this.messages);
  }


  getMessages(useCache: boolean=false) {
    this.message=undefined;    //refresh the message panel
    this.mParams.pageSize=6;
    this.service.setParams(this.mParams);

    this.service.getMessages(useCache)?.subscribe({
      next: response => {
        this.messages!=response?.data;
        this.totalCount!=response?.count;
      },
      error: error => console.log(error)
    })
  }


  getInboxMessages() {
    this.mParams=new EmailMessageSpecParams();
    this.mParams.container="inbox";
    this.service.setContainer(this.container);
    this.controlsDisabled=true;
    this.getMessages(true);
    this.toastr.info('inbox');
  }

  getOutboxMessages() {
    this.mParams.container="sent";
    this.service.setContainer(this.container);
    this.controlsDisabled=true;
    this.getMessages(true);
  }

  getDraftMessages() {
    this.mParams.container="draft";
    this.service.setContainer(this.container);
    this.controlsDisabled=false;
    this.getMessages(true);
  }

  onSearch() {
    const params = this.service.getParams();
    params.search = this.searchTerm!.nativeElement.value;
    params.pageIndex = 1;
    this.getMessages(true);
  }

  onReset() {
    this.searchTerm!.nativeElement.value = '';
    this.mParams = new EmailMessageSpecParams();
    this.controlsDisabled=this.mParams.container!=='draft';
    this.getMessages();
  }

  onSortSelected(sort: string) {
    this.mParams.sort = sort;
    this.getMessages();
  }
  
  onMsgSelected(msgId: number) {
    this.mParams.id=msgId;
    this.getMessages();
  }

  saveandclose() {

  }

  sendMessage() {

    this.service.sendMessage(this.message!).subscribe((response: any) => {
      this.toastr.success('message sent');
    }, (error: any) => {
      console.log('send message error', error);
      this.toastr.error('failed to send the email message', error);
    })
  }

  
  deleteMessage(id: number) {
    this.confirmService.confirm('Confirm delete message', 'This cannot be undone').subscribe((result: any) => {
      if (result) {
        this.service.deleteMessage(id).subscribe(() => {
          this.messages.splice(this.messages.findIndex(m => m.id === id), 1);
        })
      }
    })

  }

  
  setMessageFromUsername(msg: IMessage) {
    this.message=msg;
  }
  setMessageFromEmail(msg: IMessage) {
    this.message=msg;
  }

  onPageChanged(event: any) {
    if (this.pageIndex !== event) {
      this.pageIndex = event;
      this.mParams.pageIndex=event;
      this.getMessages(true);
    }
  }

  editorConfig: AngularEditorConfig = {
    editable: !this.controlsDisabled,
      spellcheck: true,
      height: 'auto',
      minHeight: '0',
      maxHeight: 'auto',
      width: 'auto',
      minWidth: '0',
      translate: 'yes',
      enableToolbar: true,
      showToolbar: true,
      placeholder: 'Enter text here...',
      defaultParagraphSeparator: '',
      defaultFontName: '',
      defaultFontSize: '',
      fonts: [
        {class: 'arial', name: 'Arial'},
        {class: 'times-new-roman', name: 'Times New Roman'},
        {class: 'calibri', name: 'Calibri'},
        {class: 'comic-sans-ms', name: 'Comic Sans MS'}
      ],
      customClasses: [
      {
        name: 'quote',
        class: 'quote',
      },
      {
        name: 'redText',
        class: 'redText'
      },
      {
        name: 'titleText',
        class: 'titleText',
        tag: 'h1',
      },
    ]
    
  };


}

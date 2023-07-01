import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IDeploymentPendingDto } from 'src/app/shared/dtos/process/deploymentPendingDto';

@Component({
  selector: 'app-deploy-item',
  templateUrl: './deploy-item.component.html',
  styleUrls: ['./deploy-item.component.css']
})
export class DeployItemComponent implements OnInit {

  @Input() dep: IDeploymentPendingDto | undefined;
  @Output() editDepEvent = new EventEmitter<IDeploymentPendingDto>();
  @Output() addNewEvent = new EventEmitter<IDeploymentPendingDto>();

  constructor() { }

  ngOnInit(): void {
  }

  editViewDeploysOfCVRefId() {
    this.editDepEvent.emit(this.dep);
  }

  addNew() {
    this.addNewEvent.emit(this.dep);
  }
}

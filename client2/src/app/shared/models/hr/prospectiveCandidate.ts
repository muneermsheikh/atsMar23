export interface IProspectiveCandidate
{
     id: number;
     checked: boolean;
     source: string;
     resumeId: string;
     nationality: string;
     address: string;
     city: string;
     date: Date;
     candidateName: string;
     categoryRef: string;
     orderItemId: number;
     age: string;
     phoneNo: string;
     alternatePhoneNo: string;
     email: string;
     alternateEmail: string;
     currentLocation: string;
     education: string;
     ctc: string;
     workExperience: number;
     status: string;
     newStatus: string;
     statusDate: Date;
     statusByUserId: number;
     userName: string;
     closed: boolean;
     remarks: string;
}
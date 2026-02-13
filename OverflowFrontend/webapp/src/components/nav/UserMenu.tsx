'use client';
import {Dropdown, DropdownTrigger, DropdownMenu, DropdownItem, Avatar} from "@heroui/react";
import { User } from "next-auth";
import { signOut } from "next-auth/react";

type Props = {
    user : User 
}

export default function UserMenu({user}: Props) {
  return (
    <Dropdown>
      <DropdownTrigger>
        <div className="flex items-center gap-2 cursor-pointer">
            <Avatar color="secondary" 
            size="sm"
            name={user.name?.charAt(0)} /> {user.name}
        </div>
      </DropdownTrigger>
      <DropdownMenu aria-label="Example with disabled actions" disabledKeys={["edit", "delete"]}>
     <DropdownItem key="edit">Edit Profile</DropdownItem>
     <DropdownItem key="logout"
     onClick={() => {signOut({redirectTo: "/questions"})}}>Logout</DropdownItem>
      </DropdownMenu>
    </Dropdown>
  );
}
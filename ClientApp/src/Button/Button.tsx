import './Button.css'
import React from "react";

interface ButtonProps {
	isActive?: boolean;
	children: React.ReactNode;
	onClick?: () => void;
	style?: React.CSSProperties;
}

const Button: React.FC<ButtonProps> = ({children, isActive, onClick, style}) => {
	return <button style={style} onClick={onClick} className={isActive ? 'button active' : 'button'}>{children}</button>
}

export default Button;
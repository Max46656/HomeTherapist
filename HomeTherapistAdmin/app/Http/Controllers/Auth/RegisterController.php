<?php

namespace App\Http\Controllers\Auth;

use Backpack\CRUD\app\Http\Controllers\Auth\RegisterController as BackpackRegisterController;
use Backpack\CRUD\app\Library\Auth\RegistersUsers;
use Illuminate\Support\Facades\Validator;

class RegisterController extends BackpackRegisterController
{
    use RegistersUsers;

    /**
     * Get a validator for an incoming registration request.
     *
     * @param  array  $data
     * @return \Illuminate\Contracts\Validation\Validator
     */
    protected function validator(array $data)
    {
        $user_model_fqn = config('backpack.base.user_model_fqn');
        $user = new $user_model_fqn();
        $users_table = $user->getTable();
        $email_validation = backpack_authentication_column() == 'email' ? 'email|' : '';

        return Validator::make($data, [
            'name' => 'required|max:255',
            backpack_authentication_column() => 'required|' . $email_validation . 'max:255|unique:' . $users_table,
            'password' => 'required|min:6|confirmed',
            'staff_id' => 'required|unique:' . $users_table,
            'phone' => 'required|regex:/^09\d{8}$/|unique:' . $users_table,
        ]);
    }

    /**
     * Create a new user instance after a valid registration.
     *
     * @param  array  $data
     * @return User
     */
    protected function create(array $data)
    {
        $user_model_fqn = config('backpack.base.user_model_fqn');
        $user = new $user_model_fqn();

        return $user->create([
            'name' => $data['name'],
            backpack_authentication_column() => $data[backpack_authentication_column()],
            'password' => bcrypt($data['password']),
            'staff_id' => $data['staff_id'],
            'phone' => $data['phone'],
        ]);
    }
}
